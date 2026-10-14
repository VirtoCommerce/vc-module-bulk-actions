<template>
  <VcBlade
    :title="$t('BULKACTIONS.TITLE')"
    :subtitle="scope"
    :toolbar-items="toolbar"
    :loading="loading"
    width="60%"
  >
    <VcContainer>
      <VcForm class="bulk-form">
        <VcSelect
          v-model="actionName"
          :label="$t('BULKACTIONS.ACTION')"
          :options="actionOptions"
          :clearable="false"
          :disabled="running"
          @update:model-value="onAction"
        />

        <template v-if="actionName === 'CategoryChangeBulkAction'">
          <VcField
            :label="$t('BULKACTIONS.TARGET')"
            :model-value="targetLabel || $t('BULKACTIONS.NO_TARGET')"
          />
          <div>
            <VcButton
              icon="lucide-folder-search"
              variant="secondary"
              :disabled="running"
              @click="pickTarget"
            >
              {{ $t("BULKACTIONS.PICK_TARGET") }}
            </VcButton>
          </div>
        </template>

        <template v-if="actionName === 'PropertiesUpdateBulkAction'">
          <VcSelect
            v-model="selectedNames"
            :label="$t('BULKACTIONS.PROPERTIES')"
            :options="propertyOptions"
            multiple
            searchable
            :disabled="running"
          />
          <!-- Catalog's property values editor, handed over by the Catalog list toolbar context -->
          <component
            :is="options?.propertyEditor"
            v-if="edited.properties.length && options?.propertyEditor"
            :entity="edited"
            :types="['Product', 'Variation', 'Category', 'Catalog']"
            :header="$t('BULKACTIONS.NEW_VALUES')"
            :languages="['en-US']"
          />
        </template>

        <JobProgress :job="job" />
      </VcForm>
    </VcContainer>
  </VcBlade>
</template>

<script setup lang="ts">
import { useBlade, type IBladeToolbar } from "@vc-shell/framework";
import type { Component } from "vue";
import { VcBlade, VcButton, VcContainer, VcField, VcForm, VcSelect } from "@vc-shell/framework/ui";
import { computed, onMounted, reactive, ref, watch } from "vue";
import { useI18n } from "vue-i18n";
import { http } from "../api";
import JobProgress from "../components/JobProgress.vue";
import { useJob, type JobNotification } from "../components/useJob";
import type { ListEntry, PickOptions } from "../toolbar";

// Bulk actions (VirtoCommerce.BulkActionsModule API) on the catalog list: change category, update properties
// (legacy catalogBulkActionsModule: action-list, action-change-category, action-edit-properties, bulk-action-progress)
// the rows come only through options: not restored from the URL
defineBlade({ url: "/bulk-actions", name: "BulkActions", permissions: ["bulk-actions:read"], routable: false });

/** catalog property (the bulk data endpoint returns the properties of the selection) */
interface Property {
  id?: string;
  name?: string;
  ownerName?: string;
  valueType?: string;
  values?: { value?: unknown }[];
}
interface BulkAction {
  name: string;
  contextTypeName: string;
}
const SUPPORTED = ["CategoryChangeBulkAction", "PropertiesUpdateBulkAction"];

const { t } = useI18n({ useScope: "global" });
const { options, openBlade } = useBlade<{
  catalogId: string;
  categoryId?: string;
  selection?: ListEntry[];
  /** "select all" in the list: the search criteria replace the rows (legacy) */
  criteria?: Record<string, unknown>;
  count?: number;
  propertyEditor?: Component;
  reload?: () => Promise<void>;
}>();
const { job, start } = useJob();

const actions = ref<BulkAction[]>([]);
const actionName = ref<string>();
const loading = ref(false);
const target = ref<{ catalogId?: string; categoryId?: string }>({});
const targetLabel = ref("");
const allProperties = ref<Property[]>([]);
const selectedNames = ref<string[]>([]);
const edited = reactive<{ properties: Property[] }>({ properties: [] });
const running = computed(() => !!job.value && !job.value.finished);

const actionOptions = computed(() =>
  actions.value
    .filter((a) => SUPPORTED.includes(a.name))
    .map((a) => ({ id: a.name, title: t(`BULKACTIONS.ACTIONS.${a.name}`) })),
);
const propertyOptions = computed(() =>
  allProperties.value.map((p) => ({ id: key(p), title: `${p.name} (${p.ownerName ?? ""})` })),
);
const key = (p: Property) => `${p.id ?? ""}|${p.name}`;

const selection = computed(() => options.value?.selection ?? []);
const scope = computed(() =>
  options.value?.criteria
    ? t("BULKACTIONS.SCOPE_ALL", { count: options.value?.count ?? "" })
    : t("BULKACTIONS.SCOPE_SELECTED", { count: selection.value.length }),
);
// nothing selected: nothing to run on (legacy canExecuteMethod: isItemsChecked)
const hasItems = computed(() => !!options.value?.criteria || selection.value.length > 0);

// selected rows, or the list's search (legacy select-all → searchCriteria)
const dataQuery = computed(() =>
  options.value?.criteria
    ? { searchCriteria: options.value.criteria }
    : { listEntries: selection.value.map((e) => ({ id: e.id, type: e.type, catalogId: e.catalogId, name: e.name })) },
);
const context = () => {
  const action = actions.value.find((a) => a.name === actionName.value)!;
  return { actionName: action.name, contextTypeName: action.contextTypeName, dataQuery: dataQuery.value };
};

async function busy<T>(fn: () => Promise<T>) {
  loading.value = true;
  try {
    return await fn();
  } finally {
    loading.value = false;
  }
}

async function onAction() {
  if (actionName.value === "PropertiesUpdateBulkAction" && !allProperties.value.length) {
    const data = await busy(() => http<{ properties?: Property[] }>("POST", "/api/bulk/actions/data", { ...context(), properties: [] }));
    allProperties.value = data?.properties ?? [];
  }
}

// the edited copies of the chosen properties, with empty values (Booleans default to false, legacy)
watch(selectedNames, (names) => {
  edited.properties = names.map((n) => {
    const kept = edited.properties.find((p) => key(p) === n);
    if (kept) return kept;
    const p = allProperties.value.find((x) => key(x) === n)!;
    return { ...p, values: p.valueType === "Boolean" ? [{ value: false }] : [] };
  });
});

function pickTarget() {
  openBlade({
    name: "CatalogBrowse",
    options: {
      pick: {
        title: t("BULKACTIONS.PICK_TARGET"),
        mode: "folder",
        onPick: (_, folder) => {
          if (folder.catalog?.isVirtual) throw new Error(t("BULKACTIONS.REAL_ONLY"));
          target.value = { catalogId: folder.catalogId, categoryId: folder.categoryId };
          targetLabel.value = folder.catalog?.name ?? "";
          if (folder.categoryId) targetLabel.value += ` / ${folder.categoryId}`;
        },
      } satisfies PickOptions,
    },
  });
}

const canRun = computed(() => {
  if (!hasItems.value) return false;
  if (actionName.value === "CategoryChangeBulkAction") return !!target.value.catalogId;
  if (actionName.value === "PropertiesUpdateBulkAction") return edited.properties.length > 0;
  return false;
});

const toolbar = computed<IBladeToolbar[]>(() => [
  {
    id: "start",
    title: t("BULKACTIONS.START"),
    icon: "lucide-play",
    permissions: "bulk-actions:execute",
    disabled: !canRun.value || running.value,
    async clickHandler() {
      const extra =
        actionName.value === "CategoryChangeBulkAction" ? target.value : { properties: edited.properties };
      start(await busy(() => http<JobNotification>("POST", "/api/bulk/actions", { ...context(), ...extra })));
    },
  },
  {
    id: "cancel",
    title: t("BULKACTIONS.CANCEL"),
    icon: "lucide-circle-stop",
    isVisible: running.value,
    async clickHandler() {
      await http("DELETE", `/api/bulk/actions?jobId=${encodeURIComponent(job.value!.jobId!)}`);
    },
  },
]);

watch(
  () => job.value?.finished,
  (finished) => finished && options.value?.reload?.(),
);

onMounted(async () => {
  actions.value = await busy(() => http<BulkAction[]>("GET", "/api/bulk/actions"));
  actionName.value = actionOptions.value[0]?.id;
  await onAction();
});
</script>

<style lang="scss">
.bulk-form {
  padding: 20px 22px 36px;
  display: flex;
  flex-direction: column;
  gap: 16px;
}
</style>
