import { JobsClient, PushNotificationClient, useApiClient } from "@vc-shell/framework";
import { onUnmounted, ref } from "vue";

/** Push notification of a background job (CSV export/import, bulk action). */
export interface JobNotification {
  id?: string;
  title?: string;
  description?: string;
  finished?: string | Date | null;
  totalCount?: number;
  processedCount?: number;
  errorCount?: number;
  errors?: string[];
  downloadUrl?: string;
  jobId?: string;
}

/**
 * Follows a job's push notification by polling the platform's notification search
 * (the AngularJS blades listened to SignalR instead).
 * ponytail: 1.5s polling; switch to useNotifications() if many jobs run at once.
 */
export function useJob() {
  const { getApiClient: getJobsClient } = useApiClient(JobsClient);
  const { getApiClient: getNotificationsClient } = useApiClient(PushNotificationClient);
  const job = ref<JobNotification>();
  let timer: ReturnType<typeof setTimeout> | undefined;
  let checkState = true;

  async function poll() {
    if (!job.value?.id || job.value.finished) return;
    try {
      // the job's state first: a job in a final state has already sent its last notification, so one that
      // still reads unfinished below is dead (failed before reporting, or lost in a restart: unknown ids read completed)
      let ended: { state?: string; completed?: boolean } | undefined;
      if (job.value.jobId && checkState) {
        try {
          ended = await (await getJobsClient()).getStatus(job.value.jobId);
        } catch (e) {
          const status = (e as { status?: number }).status;
          if (status === 403 || status === 404) checkState = false; // no background jobs access (or API): the notification only
        }
      }
      const res = await (await getNotificationsClient()).searchPushNotification({ ids: [job.value.id], take: 1 });
      const fresh = res.notifyEvents?.find((n) => n.id === job.value?.id) as JobNotification | undefined;
      if (fresh) job.value = { ...job.value, ...fresh };
      if (ended?.completed && !job.value.finished) {
        const error = `Background job #${job.value.jobId}: ${ended.state || "lost"}`;
        job.value = { ...job.value, finished: new Date(), description: error, errors: [...(job.value.errors ?? []), error], errorCount: (job.value.errorCount ?? 0) + 1 };
      }
    } catch {
      // transient (e.g. the platform restarting): the next tick retries
    } finally {
      if (!job.value?.finished) timer = setTimeout(poll, 1500);
    }
  }

  function start(notification: JobNotification) {
    clearTimeout(timer);
    checkState = true;
    job.value = notification;
    timer = setTimeout(poll, 1000);
  }

  onUnmounted(() => clearTimeout(timer));
  return { job, start };
}
