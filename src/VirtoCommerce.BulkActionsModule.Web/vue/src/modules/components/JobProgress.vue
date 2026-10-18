<template>
  <VcCard
    v-if="job"
    :header="job.title || $t('BULKACTIONS.JOB.TITLE')"
  >
    <div class="job-card">
      <div>{{ job.description }}</div>
      <VcProgress
        :value="percent"
        :variant="job.finished ? 'default' : 'striped'"
      />
      <div class="job-card__hint">
        {{ $t("BULKACTIONS.JOB.PROGRESS", { processed: job.processedCount ?? 0, total: job.totalCount ?? 0, errors: job.errorCount ?? 0 }) }}
      </div>
      <a
        v-if="job.downloadUrl"
        :href="job.downloadUrl"
        target="_blank"
        rel="noopener"
      >{{ $t("BULKACTIONS.JOB.DOWNLOAD") }}</a>
      <ul
        v-if="job.errors?.length"
        class="job-card__errors"
      >
        <li
          v-for="(e, i) in job.errors"
          :key="i"
        >
          {{ e }}
        </li>
      </ul>
    </div>
  </VcCard>
</template>

<script setup lang="ts">
import { VcCard, VcProgress } from "@vc-shell/framework/ui";
import { computed } from "vue";
import type { JobNotification } from "./useJob";

const props = defineProps<{ job?: JobNotification }>();
const percent = computed(() => {
  if (props.job?.finished) return 100;
  return props.job?.totalCount ? Math.round(((props.job.processedCount ?? 0) / props.job.totalCount) * 100) : 0;
});
</script>

<style lang="scss">
.job-card {
  padding: 16px;
  display: flex;
  flex-direction: column;
  gap: 16px;
  &__hint {
    color: var(--neutrals-500);
    font-size: 13px;
  }
  &__errors {
    margin: 0;
    padding-inline-start: 18px;
    color: var(--danger-500);
    font-size: 13px;
    max-height: 240px;
    overflow: auto;
  }
}
</style>
