# Layout & Navigation Components Reference

## Table of Contents

- [Accordion](#accordion) - Expandable/collapsible content sections
- [Card](#card) - Flexible container with header, content, and footer
- [Collapsible](#collapsible) - Show/hide content with animated transitions
- [List](#list) - Selection list with single/multi-select, search, grouping, and hierarchical modes
- [Tabs](#tabs) - Organize content into tabbed sections
- [Table](#table) - Interactive data table with sorting, filtering, and multi-select
- [TablePagination](#tablepagination) - Pagination controls for tables
- [Stepper](#stepper) - Multi-step process indicator
- [Sidenav](#sidenav) - Side navigation bar with hierarchical links, search, and user menu

---

## Accordion

Expandable/collapsible content sections for organizing large amounts of information.

### Basic Usage

```vue
<template>
  <spr-accordion :accordion-items="accordionItems">
    <template #item1> Item1 content </template>
    <template #item2> Item2 content </template>
    <template #item3> Item3 content </template>
  </spr-accordion>
</template>

<script lang="ts" setup>
import { ref } from 'vue';

const accordionItems = ref([
  {
    title: 'Accordion Item 1',
    subtitle: 'Description text for item 1',
    collapseId: 'item1',
  },
  {
    title: 'Accordion Item 2',
    subtitle: 'Description text for item 2',
    collapseId: 'item2',
  },
  {
    title: 'Accordion Item 3',
    subtitle: 'Description text for item 3',
    collapseId: 'item3',
  },
]);
</script>
```

### Props

| Name | Type | Default | Description |
|------|------|---------|-------------|
| accordionItems | `Array<{ title: string, subtitle?: string, collapseId: string }>` | `[]` | Array of objects defining the accordion items. Each item needs `title`, optional `subtitle`, and `collapseId` (used to match slot names). |
| alwaysOpen | `boolean` | `false` | When `true`, multiple accordion items can be expanded simultaneously. |
| isDefaultOpen | `boolean` | `false` | When `true`, all items are expanded on mount. Only works when `alwaysOpen` is `true`. |
| accordionTrigger | `'header' \| 'button'` | `'button'` | Determines which element acts as the expand/collapse trigger. `header` makes the entire header clickable; `button` limits it to the toggle button. |
| bordered | `boolean` | `true` | When `true`, the accordion has a border and rounded corners. |

### Slots

| Name | Description |
|------|-------------|
| `${collapseId}` | Dynamic slots corresponding to each accordion item's `collapseId`. Use to add content inside each panel. |

### AccordionItem Interface

```typescript
interface AccordionItem {
  title: string;
  subtitle?: string;
  collapseId: string;
}
```

---

## Card

A flexible container with optional header, content, and footer used to group related information.

### Basic Usage

```vue
<template>
  <spr-card title="Card Title">
    <template #content>
      <div>Card content</div>
      <div>Lorem ipsectetur adipiscing elit. Sed etiam, sed etiam.</div>
    </template>
    <template #footer>
      <div class="spr-ms-auto spr-flex spr-items-center spr-gap-size-spacing-3xs">
        <spr-button variant="secondary">secondary</spr-button>
        <spr-button tone="success">primary</spr-button>
      </div>
    </template>
  </spr-card>
</template>
```

### Props

| Name | Type | Default | Description |
|------|------|---------|-------------|
| id | `string` | `''` | Unique identifier for the element. |
| tone | `'plain' \| 'neutral' \| 'success' \| 'information' \| 'pending' \| 'caution' \| 'accent' \| 'danger'` | `''` | Sets the card's visual tone/color scheme. |
| title | `string` | `''` | Sets the card's title in the header section. |
| subtitle | `string` | `''` | Subtitle displayed below the title. Requires title to be visible. |
| header-icon | `string` | `''` | Iconify icon name displayed in the header. Requires title to be visible. |
| show-header | `boolean` | `true` | Controls header section visibility. |
| show-footer | `boolean` | `true` | Controls footer section visibility. |
| border-width | `string` | `'1px'` | Sets the border width of the card (any valid CSS width value). |
| border-radius-size | `'xl' \| 'lg' \| 'md' \| 'sm' \| 'xs' \| '2xs'` | `'xl'` | Sets the border radius of the card. |
| has-collapsible | `boolean` | `false` | Indicates the card is used with a collapsible component, affecting border styling. |
| is-collapsible-open | `boolean` | `false` | Tracks expanded state when used with collapsible, for border style sync. |
| has-content-padding | `boolean` | `true` | Controls whether padding is applied to the content area. |
| flexbox | `boolean` | `false` | When `true`, applies flexbox layout (column direction) to the card. |
| customBorderSize | `string \| null` | `null` | Custom border size override. |

### Slots

| Name | Description |
|------|-------------|
| header | Custom header content. Displayed alongside the title if provided, or fully replaces the header if no title is set. |
| content | The main content area of the card. |
| default | Alternative slot for content. Used only if no `content` slot is provided. |
| footer | Footer content. Typically used for action buttons or summary information. |

---

## Collapsible

Show and hide content with smooth animated transitions. Commonly used for expandable sections and dropdown menus.

### Basic Usage

```vue
<template>
  <spr-button tone="success" @click="isOpen = !isOpen">Toggle Me</spr-button>
  <spr-collapsible v-model="isOpen">
    <div class="spr-p-4">Collapsible content here</div>
  </spr-collapsible>
</template>

<script setup>
import { ref } from 'vue';

const isOpen = ref(false);
</script>
```

### Props

| Name | Type | Default | Description |
|------|------|---------|-------------|
| modelValue | `boolean` | - | **Required.** Controls whether the content is expanded (`true`) or collapsed (`false`). |
| transitionDuration | `number` | `150` | Duration of the expand/collapse animation in milliseconds. |

### Events

| Name | Parameters | Description |
|------|------------|-------------|
| update:modelValue | `(value: boolean)` | Emitted when the expanded/collapsed state changes. |

### Slots

| Name | Description |
|------|-------------|
| default | The content that will be collapsed/expanded. |
| trigger | Custom trigger element. Provides `{ toggleCollapsible: () => void }` scoped prop that auto-updates the v-model. |

---

## List

A versatile selection list supporting single/multi-select, grouping, search, hierarchical structures, radio buttons, and lozenge display modes.

### Basic Usage

```vue
<template>
  <div
    :class="[
      'spr-max-h-[300px] spr-overflow-auto spr-rounded-md spr-p-2',
      'spr-border-color-weak spr-border spr-border-solid',
    ]"
  >
    <spr-list v-model="selectedItems" :menu-list="menuList" />
  </div>
</template>

<script lang="ts" setup>
import { ref } from 'vue';

const selectedItems = ref([]);

const menuList = ref([
  { text: 'Apple', value: 'apple' },
  { text: 'Banana', value: 'banana' },
  { text: 'Cherry', value: 'cherry' },
  { text: 'Date', value: 'date' },
  { text: 'Elderberry', value: 'elderberry' },
]);
</script>
```

### Props

| Name | Type | Default | Description |
|------|------|---------|-------------|
| model-value (v-model) | `MenuListType[]` | `[]` | Two-way binding for selected items containing full item objects. |
| menu-list | `MenuListType[]` | `[]` | **Required.** Array of items to display. Each item needs `text` and `value`. |
| group-items-by | `'default' \| 'A-Z' \| 'Z-A'` | `undefined` | Grouping strategy: `'default'` groups by item `group` property, `'A-Z'`/`'Z-A'` sorts alphabetically. |
| multi-select | `boolean` | `false` | Enable multi-selection mode with checkboxes. |
| pre-selected-items | `(string \| number \| Record<string, unknown>)[]` | `[]` | Pre-select items by their values. |
| searchable-menu | `boolean` | `false` | Display search input for filtering items. |
| searchable-menu-placeholder | `string` | `'Search...'` | Placeholder text for search input. |
| search-value | `string` | `''` | External search value (two-way binding). |
| menu-level | `number` | `0` | Nesting level for hierarchical lists. |
| ladderized | `boolean` | `false` | Enable hierarchical/ladderized list display. |
| disabled-local-search | `boolean` | `false` | Disable local search filtering (for server-side search). |
| loading | `boolean` | `false` | Show loading skeleton instead of items. |
| no-check | `boolean` | `false` | Hide checkmark icon in single-select mode. |
| lozenge | `boolean` | `false` | Display items as lozenges (requires `lozengeProps` on items). |
| supporting-display-text | `string` | `''` | Display custom text (e.g., "2 Selected"). |
| display-list-item-selected | `boolean` | `false` | Display count of selected items when searchable. |
| sticky-search-offset | `string \| number` | `0` | Offset for sticky search header. |
| item-icon | `string` | `''` | Default Iconify icon for all items. |
| item-icon-tone | `string` | `'plain'` | Tone for item icons: `'plain'`, `'pending'`, `'information'`, `'success'`, `'danger'`, `'neutral'`, `'caution'`. |
| item-icon-fill | `boolean` | `false` | Fill style (solid background) for item icons. |
| disabled-unselected-items | `boolean` | `false` | Disable and gray out unselected items. |
| radio-list | `boolean` | `false` | Display radio buttons for single-select mode. |
| allow-deselect | `boolean` | `false` | Allow deselection of a selected item in single-select mode. |
| allow-select-all | `boolean` | `false` | Show a "Select All" / "Unselect All" button in multi-select mode. |

### Events

| Name | Parameters | Description |
|------|------------|-------------|
| update:modelValue | `MenuListType[]` | Emitted when selection changes. Array of selected item objects. |
| update:searchValue | `string` | Emitted when search input changes. |
| get-single-selected-item | `MenuListType` | Emitted when an item is selected in single-select mode. |
| get-single-deselected-item | `MenuListType` | Emitted when an item is deselected in allow-deselect mode. |

### MenuListType Item Properties

```typescript
interface MenuListType {
  text: string;           // Display label
  value: string | number; // Unique identifier
  group?: string;         // Group name (for grouping)
  subtext?: string;       // Additional description
  icon?: string;          // Iconify icon name
  iconColor?: string;     // Tailwind class for icon color
  disabled?: boolean;     // Whether item is disabled
  sublevel?: MenuListType[]; // Child items (for ladderized lists)
  lozenge?: { label: string; tone: string; fill: boolean }; // Right-side lozenge badge
  lozengeProps?: { label: string; tone: string; fill: boolean; icon?: string }; // Full lozenge mode
}
```

---

## Tabs

Organize content into tabbed sections with support for icons, badges, and underlined style.

### Basic Usage

```vue
<template>
  <spr-tabs :list="tabs" />
</template>

<script lang="ts" setup>
import { ref } from 'vue';

const tabs = ref([
  { label: 'Tab 1' },
  { label: 'Tab 2' },
  { label: 'Tab 3' },
]);
</script>
```

### Props

| Name | Type | Default | Description |
|------|------|---------|-------------|
| list | `Array<{ label: string; icon?: string; iconFill?: string \| Component; disabled?: boolean; badge?: BadgePropTypes }>` | `[]` | Array of tab items to display. |
| underlined | `boolean` | `false` | When `true`, tabs use underline style. When `false`, tabs use button-like appearance. |
| activeTab | `string` | `''` | Sets the active tab by matching its `label`. First tab is active by default. |
| showBadge | `boolean` | `false` | When `true`, badges defined in tab items are rendered. |

### Events

| Name | Parameters | Description |
|------|------------|-------------|
| tabIndex | `(index: number)` | Emitted when a tab is selected. Returns the index of the selected tab. |

### List Item Properties

| Name | Type | Required | Description |
|------|------|----------|-------------|
| label | `string` | Yes (unless icon-only) | Text displayed in the tab. Also used to match `activeTab`. |
| icon | `string` | No | Iconify icon name to display in the tab. |
| iconFill | `string \| Component` | No | Icon displayed when the tab is active. |
| disabled | `boolean` | No | Whether the tab is disabled. |
| badge | `BadgePropTypes` | No | Badge config: `{ text: string, variant: string, size: string }`. Requires `showBadge` to be enabled. |

---

## Table

Interactive data table with sorting, filtering, multi-select, custom columns, drag-and-drop, and slot-based customization.

### Basic Usage

```vue
<template>
  <spr-table action :headers="headers" :data-table="data">
    <div>Customize your content here!</div>
    <template #action="{ row }">
      <spr-lozenge :label="row.status.title" :tone="row.status.title.toLowerCase()" />
    </template>
    <template #action-name> Status </template>
  </spr-table>
</template>

<script setup lang="ts">
import { ref } from 'vue';

const headers = ref([
  { field: 'name', name: 'Role Name', sort: true, hasAvatar: true, hasSubtext: true },
  { field: 'lastUpdate', name: 'Date', sort: true, hasAvatar: false, hasSubtext: false },
]);

const data = ref([
  {
    name: {
      title: 'Shift',
      subtext: 'Lorem ipsectetur adipiscing elit.',
      image: 'https://example.com/avatar.jpeg',
    },
    lastUpdate: {
      title: 'Nov 30, 2025',
    },
    status: {
      title: 'Success',
    },
  },
]);
</script>
```

### Props

| Name | Type | Default | Description |
|------|------|---------|-------------|
| action | `boolean` | `false` | Enables an action column at the end of the table. |
| dataTable | `Array<TableData>` | `[]` | Array of row data objects with properties matching header `field` names. |
| headers | `Array<Header>` | `[]` | Array of header config objects defining table columns. |
| emptyState | `EmptyState` | `{ description: 'No results found', subDescription: 'Try a different search term', image: 'location', size: 'large' }` | Configuration for the empty state display. |
| loading | `boolean` | `false` | Displays a loading state instead of data. |
| tableActions | `{ search: boolean, filter: boolean, option: boolean }` | `{ search: false, filter: false, option: false }` | Configuration for table action controls above the table. |
| searchModel | `string` | `''` | Two-way binding for search input (use `v-model:searchModel`). |
| sortOrder | `'asc' \| 'desc'` | `'asc'` | Default sort order. |
| variant | `'surface' \| 'white'` | `'surface'` | Controls header background color. |
| isRowClickable | `boolean` | `false` | Enables row click events. |
| fullHeight | `boolean` | `true` | Table takes full available height of its container. |
| removeHeaderOnEmpty | `boolean` | `false` | Hides headers when no data exists. |
| isMultiSelect | `boolean` | `false` | Enables multi-select with checkboxes. |
| selectedKeyId | `string` | `''` | Key in data object used as row identifier for multi-select. Required with `isMultiSelect`. |
| returnCompleteSelectedProperties | `boolean` | `false` | If `true`, emits full row data on selection; if `false`, emits only `selectedKeyId` values. |
| isDraggable | `boolean` | `false` | Enables drag-and-drop for table rows. |
| allowSelfDrag | `boolean` | `false` | Enables drag-and-drop within the same table. |
| retainSelectionOnDataChange | `boolean` | `false` | Retains selected rows when table data changes. |
| showHeaderFilter | `boolean` | `false` | Displays filter dropdowns in headers. Requires `filterList` on header objects. |

### Events

| Name | Parameters | Description |
|------|------------|-------------|
| update:searchModel | `(value: string)` | Emitted when search input changes. |
| onSort | `({ field: string, sortOrder: 'asc' \| 'desc' })` | Emitted when a sortable column header is clicked. |
| onRowClick | `(rowData: TableData, rowIndex: number)` | Emitted when a row is clicked (requires `isRowClickable`). |
| onHover | `({ active: boolean, data: TableData })` | Emitted on row mouse enter/leave. |
| update:selectedData | `(selectedItems: TableData[] \| any[])` | Emitted when selection changes in multi-select mode. |
| onDropToEmptyZone | `(event: DragOnChangeEvent['added'])` | Emitted when a dragged item is dropped into an empty state. |
| onDropChange | `(event: DragOnChangeEmit)` | Emitted when a dragged item is dropped into a populated table. |
| on-apply-filter | `(filters: object)` | Emitted when header filter is applied. |

### Slots

| Name | Description |
|------|-------------|
| default | Content displayed above the table (title/description). |
| tableActionSection | Custom content for the table actions area (buttons, controls). |
| action | Action column content per row. Props: `{ row: TableData }`. Requires `action` prop. |
| action-name | Header content for the action column. Requires `action` prop. |
| empty-state | Custom empty state content. |
| loading | Custom loading state content. |
| footer | Footer content (typically pagination). |
| [field] | Dynamic slots based on header `field` names for custom column rendering. Props: `{ row: TableData, rowIndex: number }`. |

### Exposed Functions

| Name | Description |
|------|-------------|
| clearSelectedData | Clears all selected rows in multi-select mode. |

### Header Object Properties

| Name | Type | Required | Description |
|------|------|----------|-------------|
| field | `string` | Yes | Unique column identifier mapping to data property names. |
| name | `string` | Yes | Display name for the column header. |
| sort | `boolean` | No | Whether the column is sortable. |
| hasAvatar | `boolean` | No | Display an avatar in cells (data needs `image` property). |
| hasIcon | `boolean` | No | Display an icon in cells (data needs `icon` property). |
| hasSubtext | `boolean` | No | Display subtext in cells (data needs `subtext` property). |
| hasLozengeTitle | `boolean` | No | Display title as a lozenge. |
| hasChipTitle | `boolean` | No | Display title as a chip. |
| badgeText | `string` | No | Badge text next to column header. |
| badgeVariant | `string` | No | Badge variant: `'disabled'`, `'brand'`, `'danger'`, `'information'`. |
| avatarVariant | `string` | No | Avatar variant for the column cells. |
| customTailwindClasses | `string` | No | Custom Tailwind CSS classes for header cells. |
| width | `string` | No | Column width (any valid CSS width: `'200px'`, `'25%'`, `'auto'`). |
| filterList | `MenuListType[]` | No | Filter options for header dropdown (requires `showHeaderFilter`). |

---

## TablePagination

Pagination controls for tables with row count selection, page navigation, and optional editable page input.

### Basic Usage

```vue
<template>
  <spr-table :headers="headers" :data-table="data">
    <template #footer>
      <spr-table-pagination
        v-model:selected-row-count="selectedRowCount"
        v-model:current-page="currentPage"
        :total-items="totalItems"
        :dropdown-selection="dropdownSelection"
        :version="'backend'"
        @previous="handlePrevious"
        @next="handleNext"
      />
    </template>
  </spr-table>
</template>

<script lang="ts" setup>
import { ref } from 'vue';

const totalItems = ref(100);
const currentPage = ref(1);
const selectedRowCount = ref(10);
const dropdownSelection = [
  { text: '10', value: '10' },
  { text: '20', value: '20' },
  { text: '50', value: '50' },
];

const handlePrevious = () => {
  if (currentPage.value > 1) currentPage.value--;
};

const handleNext = () => {
  if (currentPage.value * selectedRowCount.value < totalItems.value) currentPage.value++;
};
</script>
```

### Props

| Name | Type | Default | Description |
|------|------|---------|-------------|
| selectedRowCount | `number` | `10` | **Required.** Number of rows to display per page. |
| totalItems | `number` | `1` | **Required.** Total number of items in the dataset. |
| currentPage | `number` | `1` | **Required.** Current active page number. |
| dropdownSelection | `Array<{ text: string; value: string }>` | `[{ text: 10, value: 10 }, ...]` | **Required.** Available options for rows per page. |
| bordered | `boolean` | `true` | Whether to show border around the pagination component. |
| editableCurrentPage | `boolean` | `false` | Enable direct input of page number. |
| showNumberOfRowsDropdown | `boolean` | `true` | When `false`, hides the rows-per-page dropdown. |

### Events

| Name | Parameters | Description |
|------|------------|-------------|
| update:selectedRowCount | `(value: number)` | Emitted when rows per page changes. |
| update:currentPage | `(value: number)` | Emitted when current page changes. |
| previous | - | Emitted when previous page button is clicked. |
| next | - | Emitted when next page button is clicked. |

---

## Stepper

Multi-step process indicator showing the status of each step (completed, active, pending).

### Basic Usage

```vue
<template>
  <spr-stepper :steps="steps" />
</template>

<script setup lang="ts">
import { ref } from 'vue';

const steps = ref([
  { number: 1, label: 'Step 1', status: 'completed', description: 'Description' },
  { number: 2, label: 'Step 2', status: 'active' },
  { number: 3, label: 'Step 3', status: 'pending' },
  { number: 4, label: 'Step 4', status: 'pending' },
]);
</script>
```

### Props

| Name | Type | Default | Description |
|------|------|---------|-------------|
| variant | `'horizontal' \| 'vertical'` | `'vertical'` | Layout direction of the stepper. |
| hasLines | `boolean` | `false` | Show connector lines between steps. Lines extend to fill available space. |
| steps | `StepPropTypes[]` | `[]` | Array of step config objects (see Step Props below). |
| type | `'compact' \| 'solid'` | `'compact'` | Visual style. `'compact'` uses outline indicators; `'solid'` uses filled backgrounds. |

### Events

| Name | Parameters | Description |
|------|------------|-------------|
| click | `(evt: MouseEvent)` | Emitted when a step is clicked. |

### Step Item Properties

| Name | Type | Required | Description |
|------|------|----------|-------------|
| number | `number` | Yes | Step number displayed in the indicator. |
| label | `string` | No | Text label for the step. |
| status | `'completed' \| 'active' \| 'pending'` | No (defaults to `'pending'`) | Current state of the step. |
| description | `string` | No | Optional description text below the label. |

---

## Sidenav

Customizable side navigation bar with logo, hierarchical navigation links, quick actions, search, notifications, requests, and user menu. Automatically adapts to mobile screens (< 1024px) with a collapsible hamburger menu.

### Mobile Behavior

On screens smaller than 1024px, the Sidenav automatically switches to mobile mode:
- Displays a hamburger menu button in the header
- Navigation links appear in a slide-down panel when expanded
- User menu, notifications, and requests are accessible from the mobile header
- The panel closes when clicking outside or selecting a navigation item

### Basic Usage

```vue
<template>
  <spr-sidenav
    :nav-links="navLinks"
    :quick-actions="quickActions"
    :active-nav="activeNav"
    has-search
    notification-count="3"
    request-count="5"
    :user-menu="userMenu"
    @get-navlink-item="handleNavClick"
    @search="handleSearch"
    @notifications="handleNotifications"
    @requests="handleRequests"
  >
    <template #logo-image>
      <img src="@/assets/images/logo.svg" alt="logo" />
    </template>
  </spr-sidenav>
</template>

<script lang="ts" setup>
import { ref } from 'vue';

const activeNav = ref({ parentNav: 'Home', menu: 'Dashboard', submenu: '' });

const navLinks = ref({
  top: [
    {
      parentLinks: [
        {
          title: 'Home',
          icon: 'ph:house-simple',
          menuLinks: [
            {
              menuHeading: 'Management',
              items: [
                {
                  title: 'Dashboard',
                  redirect: { openInNewTab: false, isAbsoluteURL: false, link: '/dashboard' },
                },
              ],
            },
          ],
        },
      ],
    },
  ],
  bottom: [],
});

const quickActions = ref([]);
const userMenu = ref({
  name: 'John Doe',
  email: 'john@example.com',
  profileImage: 'https://example.com/avatar.jpeg',
  items: [
    {
      title: 'My Profile',
      icon: 'ph:user',
      redirect: { openInNewTab: false, isAbsoluteURL: false, link: '/profile' },
    },
    {
      title: 'Logout',
      icon: 'ph:sign-out',
      redirect: { openInNewTab: false, isAbsoluteURL: false, link: '/logout' },
    },
  ],
});

const handleNavClick = (navItem) => { /* handle navigation */ };
const handleSearch = (event) => { /* handle search */ };
const handleNotifications = (event) => { /* handle notifications */ };
const handleRequests = (event) => { /* handle requests */ };
</script>
```

### Props

| Name | Type | Default | Description |
|------|------|---------|-------------|
| quick-actions | `QuickAction[]` | `[]` | Quick action menu with grouped items. Each action has title, description, icon, and redirect. |
| has-search | `boolean` | `false` | Controls visibility of the search button. |
| active-nav | `{ parentNav: string; menu: string; submenu: string }` | `{ parentNav: '', menu: '', submenu: '' }` | Sets the active navigation state to highlight items. |
| nav-links | `NavLinks` | `{ top: [], bottom: [] }` | Navigation structure with `top` and `bottom` sections containing hierarchical parent/menu/submenu links. |
| notification-count | `string \| number` | `''` | Notification badge count. Use `0` to show icon without badge, empty string to hide. |
| request-count | `string \| number` | `''` | Request badge count. Use `0` to show icon without badge, empty string to hide. |
| user-menu | `UserMenu \| false` | `false` | User avatar and menu at bottom of sidenav. Set to `false` to hide. |
| is-notif-active | `boolean` | `false` | Whether notification icon appears in active (filled) state. |
| is-request-active | `boolean` | `false` | Whether request icon appears in active (filled) state. |
| is-nav-api | `boolean` | `false` | When `true`, expects navigation data in API format and transforms it internally. |

### Events

| Name | Parameters | Description |
|------|------------|-------------|
| get-navlink-item | `objectItem: object` | Emitted when a navigation link is clicked. Returns the item with redirect and active nav info. |
| search | `event: string` ('search-triggered') | Emitted when search button is clicked. |
| notifications | `event: string` ('notifications-triggered') | Emitted when notifications button is clicked. |
| requests | `event: string` ('requests-triggered') | Emitted when requests button is clicked. |

### Slots

| Name | Description |
|------|-------------|
| logo-image | Custom logo image at the top of the sidenav. Typically an `<img>` tag (optimal size: 24px x 24px). |

### Types

```typescript
type QuickAction = {
  menuHeading: string;
  items: QuickActionItem[];
};

type QuickActionItem = {
  title: string;
  description: string;
  icon: string;
  iconBgColor: string; // 'green' or 'purple'
  redirect: Redirect;
  hidden: boolean;
};

type NavLinks = {
  top: { parentLinks: ParentLinkItem[] }[];
  bottom: { parentLinks: ParentLinkItem[] }[];
};

type ParentLinkItem = {
  title: string;
  icon: string;
  link?: string;
  redirect?: Redirect;
  menuLinks: MenuLink[];
  submenuLinks?: SubmenuLink[];
  hidden?: boolean;
  attributes?: Attributes[];
};

type MenuLink = {
  menuHeading: string;
  items: MenuLinkItem[] | ParentLinkItem[];
};

type MenuLinkItem = {
  title: string;
  hidden: boolean;
  redirect: Redirect;
  submenuLinks: SubmenuLink[];
  attributes?: Attributes[];
};

type SubmenuLink = {
  subMenuHeading: string;
  items: SubmenuLinkItem[] | ParentLinkItem[];
};

type SubmenuLinkItem = {
  title: string;
  hidden: boolean;
  redirect: Redirect;
  attributes?: Attributes[];
};

type Redirect = {
  openInNewTab: boolean;
  isAbsoluteURL: boolean;
  link: string;
};

type UserMenu = {
  name: string;
  email: string;
  profileImage: string;
  items: UserMenuItem[];
};

type UserMenuItem = {
  title: string;
  icon: string;
  hidden: boolean;
  redirect: Redirect;
};

type Attributes = {
  name: string;
  value: unknown | string | number | boolean | AttrLozenge;
};

type AttrLozenge = {
  label: string;
  tone?: string; // 'danger' | 'information' | 'plain' | 'pending' | 'success' | 'neutral' | 'caution'
};
```
