import type { Component } from "vue";

// The Catalog list's toolbar extension point "catalog-items-toolbar" (contract documented in the Catalog
// plugin, vc-module-catalog .../vue/src/modules/pages/browse.vue). Types repeated here: the contract is by shape.

export interface Catalog {
  id?: string;
  name?: string;
  isVirtual?: boolean;
}

export interface ListEntry {
  id?: string;
  type?: string;
  name?: string;
  code?: string;
  catalogId?: string;
}

export interface CatalogToolbarContext {
  catalog?: Catalog;
  category?: { id: string; name?: string };
  selection: ListEntry[];
  allSelected: boolean;
  totalCount: number;
  searchCriteria: Record<string, unknown>;
  /** Catalog's property values editor (props: entity { properties }, types, header, languages) */
  propertyEditor: Component;
  openBlade: (event: { name: string; param?: string; options?: Record<string, unknown> }) => Promise<void>;
  reload: () => Promise<void>;
}

export interface CatalogToolbarCommand {
  title: string;
  icon?: string;
  permissions?: string | string[];
  isVisible?: (ctx: CatalogToolbarContext) => boolean;
  disabled?: (ctx: CatalogToolbarContext) => boolean;
  onClick: (ctx: CatalogToolbarContext) => unknown;
}

/** options.pick of the Catalog's "CatalogBrowse" blade: a catalog/category picker */
export interface PickOptions {
  title?: string;
  /** "entries": pick selected rows; "folder": pick the current catalog/category */
  mode: "entries" | "folder";
  /** limit the selectable rows (e.g. ["product"]) */
  types?: string[];
  onPick: (
    entries: ListEntry[],
    folder: { catalogId?: string; categoryId?: string; catalog?: Catalog },
    all?: Record<string, unknown>,
  ) => unknown;
}
