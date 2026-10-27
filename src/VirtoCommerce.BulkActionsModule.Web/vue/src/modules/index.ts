import * as blades from "./pages";
import * as locales from "./locales";
import { defineAppModule, useExtensionPoint } from "@vc-shell/framework";
import { markRaw } from "vue";
import type { CatalogToolbarCommand } from "./toolbar";

// "Bulk actions" on the Catalog list toolbar (legacy: catalogBulkActionsModule's toolbarService registration)
useExtensionPoint("catalog-items-toolbar").add({
  id: "bulk-actions",
  priority: 30,
  component: markRaw({ render: () => null }),
  meta: {
    title: "BULKACTIONS.TITLE",
    icon: "lucide-layers",
    permissions: "bulk-actions:read",
    // legacy canExecuteMethod: isItemsChecked
    disabled: (ctx) => !ctx.allSelected && !ctx.selection.length,
    onClick: (ctx) =>
      ctx.openBlade({
        name: "BulkActions",
        options: {
          catalogId: ctx.catalog?.id,
          categoryId: ctx.category?.id,
          selection: ctx.selection,
          // legacy: "select all" sends the search criteria instead of the rows
          criteria: ctx.allSelected ? ctx.searchCriteria : undefined,
          count: ctx.allSelected ? ctx.totalCount : ctx.selection.length,
          propertyEditor: ctx.propertyEditor,
          reload: ctx.reload,
        },
      }),
  } satisfies CatalogToolbarCommand,
});

export default defineAppModule({
  blades,
  locales,
  // legacy: header notification template for the job (progress + error count)
  notifications: { BulkActionPushNotification: { toast: { mode: "progress" } } },
});
