# Overlay & Feedback Components Reference

## Table of Contents

- [Modal](#modal) - Dialog overlay for alerts, prompts, and forms
- [Tooltip](#tooltip) - Hover-triggered informational popup
- [Dropdown](#dropdown) - Menu of options triggered by a click element
- [Popper](#popper) - Utility for positioning floating elements relative to references
- [Snackbar](#snackbar) - Toast notification for messages and actions
- [Sidepanel](#sidepanel) - Slide-in panel for supplementary content
- [StackingSidepanel](#stackingsidepanel) - Multiple sidepanels displayed side-by-side
- [FloatingAction](#floatingaction) - Fixed action bar at the bottom of the screen

---

## Modal

A versatile dialog overlay for displaying important information, alerts, or prompts without leaving the current page.

### Basic Usage

```vue
<template>
  <spr-button tone="success" @click="modalModel = true">Open Modal</spr-button>

  <spr-modal v-model="modalModel">
    <p class="spr-text-center">This is a sample modal</p>
  </spr-modal>
</template>

<script setup lang="ts">
import { ref } from 'vue';

const modalModel = ref<boolean>(false);
</script>
```

### Props

| Name | Type | Default | Description |
|------|------|---------|-------------|
| modelValue | boolean | false | Controls visibility of the modal. Used with v-model for two-way binding. |
| title | string | '' | Text to display in the modal header. If both this prop and the header slot are used, the slot takes precedence. |
| size | 'sm' \| 'md' \| 'lg' \| 'xl' \| 'xxl' | 'md' | Width of the modal: sm (360-480px), md (480-720px), lg (720-960px), xl (900-1200px), xxl (1200-1400px). |
| closeButtonX | boolean | true | When true, displays an X close button in the top-right corner of the header. |
| contentPadding | boolean | true | Controls whether the modal content area has padding. |
| hasFooter | boolean | true | Controls whether the modal renders a footer section. |
| staticBackdrop | boolean | false | When true, prevents closing the modal by clicking the backdrop. Triggers a bounce animation instead. |

### Events

| Name | Parameters | Description |
|------|------------|-------------|
| update:modelValue | (value: boolean) | Emitted when modal visibility changes. Used for v-model binding. |

### Slots

| Name | Description |
|------|-------------|
| default | Main content area of the modal. Padding controlled by `contentPadding` prop. |
| header | Custom header content. Takes precedence over the `title` prop. |
| footer | Custom footer content. Only rendered when `hasFooter` is true. |

---

## Tooltip

A simple component that displays informational text when hovered over a target element.

### Basic Usage

```vue
<spr-tooltip text="This is my tooltip text">
    <!-- Your component here -->
</spr-tooltip>
```

### Props

| Name | Type | Default | Description |
|------|------|---------|-------------|
| text | string | '' | The content to display inside the tooltip. |
| placement | string | 'top' | Position relative to target: 'top', 'top-start', 'top-end', 'bottom', 'bottom-start', 'bottom-end', 'left', 'left-start', 'left-end', 'right', 'right-start', 'right-end'. |
| distance | number | 6 | Distance in pixels between the tooltip and the target element. |
| hasMaxWidth | boolean | true | When true, tooltip has a maximum width of 280px. |
| fitContent | boolean | true | When true, tooltip width fits its content. When false, takes full width of parent. |
| showTriggers | string \| string[] | 'hover' | Events that trigger the tooltip to show: 'hover', 'focus', 'click', 'touch'. |
| hideTriggers | string \| string[] | 'hover' | Events that trigger the tooltip to hide: 'hover', 'focus', 'click', 'touch'. |
| autoHide | boolean | false | When true, tooltip auto-hides when cursor leaves the tooltip area. |

### Slots

| Name | Description |
|------|-------------|
| default | The content that triggers the tooltip on interaction. |
| popper-content | Custom HTML content inside the tooltip. If both `text` prop and this slot are provided, the text prop displays first. |

---

## Dropdown

A flexible menu of options displayed when a user interacts with a trigger element, used for navigation menus and action lists.

### Basic Usage

```vue
<template>
  <spr-dropdown
    id="my-dropdown"
    v-model="selectedValue"
    :menu-list="menuList"
    width="100px"
    popper-width="200px"
  >
    <spr-button class="spr-w-full" tone="success" has-icon>
      <span>Button</span>
      <Icon icon="ph:caret-down" />
    </spr-button>
  </spr-dropdown>
</template>

<script lang="ts" setup>
import { ref } from 'vue';
import { Icon } from '@iconify/vue';

const selectedValue = ref('');
const menuList = ref([
  { text: 'Google', value: 'https://www.google.com' },
  { text: 'GitHub', value: 'https://github.com' },
  { text: 'Gmail', value: 'https://mail.google.com' },
]);
</script>
```

### Props

| Name | Type | Default | Description |
|------|------|---------|-------------|
| id | string | - | **Required.** Unique identifier for the dropdown. |
| model-value | string \| number \| object \| Array | [] | The selected value(s), bound with v-model. |
| menu-list | array | [] | List of options. Array of objects with `text`/`value`, strings, or custom objects. |
| searchable-menu | boolean | false | Enables search functionality in the dropdown menu. |
| text-field | string | 'text' | Property name to use for display text in custom objects. |
| value-field | string | 'value' | Property name to use for the value in custom objects. |
| search-string | string | '' | Search term to filter dropdown options. |
| multi-select | boolean | false | Allows selecting multiple options. |
| placement | string | 'bottom' | Position of dropdown menu: 'auto', 'auto-start', 'auto-end', 'top', 'top-start', 'top-end', 'right', 'right-start', 'right-end', 'bottom', 'bottom-start', 'bottom-end', 'left', 'left-start', 'left-end'. |
| distance | number | 6 | Distance between trigger element and dropdown menu. |
| group-items-by | 'A-Z' \| 'Z-A' | - | Groups dropdown items alphabetically. |
| wrapper-position | string | 'relative' | CSS position value for the dropdown wrapper. |
| width | string | '100%' | Width of the dropdown wrapper (including trigger element). |
| auto-hide | boolean | true | Auto-hides dropdown when clicking outside. |
| triggers | ('click' \| 'hover' \| 'focus' \| 'touch')[] | ['click'] | Events that trigger the dropdown to open. |
| popper-triggers | ('click' \| 'hover' \| 'focus' \| 'touch')[] | [] | Events that trigger the popper element to open. |
| popper-strategy | 'absolute' \| 'fixed' | 'absolute' | Positioning strategy. Use 'fixed' inside modals (set `wrapper-position="initial"` too). |
| popper-width | string | '100%' | Width of the dropdown menu. |
| popper-inner-width | string | 'unset' | Width of the inner content area of the dropdown menu. |
| popper-container | string | '' | Selector for the container where the popper mounts. |
| disabled | boolean | false | Disables the dropdown. |
| ladderized | boolean | false | Enables hierarchical/nested dropdown options. |
| remove-current-level-in-back-label | boolean | false | Controls back label behavior in ladderized dropdowns. |
| no-check-in-list | boolean | false | Hides checkmark icons in the dropdown list. |
| lozenge | boolean | false | Enables lozenge list display. |
| no-padding | boolean | false | Removes padding from the popper content. |

### Events

| Name | Parameters | Description |
|------|------------|-------------|
| update:modelValue | The new selected value(s) | Emitted when the selected value(s) change. |
| infinite-scroll-trigger | boolean | Emitted when user scrolls to the bottom of the list (for lazy loading). |
| popper-state | boolean | Emitted when the popper opens or closes. |

### Methods

| Name | Parameters | Description |
|------|------------|-------------|
| showDropdown() | None | Programmatically opens the dropdown menu. |
| hideDropdown() | None | Programmatically closes the dropdown menu. |

### Slots

| Name | Description |
|------|-------------|
| default | The trigger element that opens the dropdown (typically a button, chips, or lozenge). |
| popper | Custom elements for popper content, replacing the default menu list. |

---

## Popper

A low-level utility for positioning floating elements relative to reference elements, used for tooltips, dropdowns, and custom overlay content.

### Basic Usage

```vue
<template>
  <spr-popper
    id="basic-example"
    distance="4"
    placement="bottom"
    :triggers="[]"
    :popper-hide-triggers="[]"
    :auto-hide="false"
    :delay="0"
  >
    <spr-button>Click for Menu</spr-button>
    <template #content>
      <div class="spr-bg-white spr-rounded-lg spr-p-4 spr-shadow-lg">
        <h3 class="spr-mb-2 spr-text-lg spr-font-medium">Menu Options</h3>
        <ul class="spr-space-y-2">
          <li>Profile</li>
          <li>Settings</li>
          <li>Logout</li>
        </ul>
      </div>
    </template>
  </spr-popper>
</template>
```

### Props

| Name | Type | Default | Description |
|------|------|---------|-------------|
| id | string | - | **Required.** Unique identifier for the popper container. |
| placement | string | 'bottom' | Position relative to reference: 'auto', 'auto-start', 'auto-end', 'top', 'top-start', 'top-end', 'right', 'right-start', 'right-end', 'bottom', 'bottom-start', 'bottom-end', 'left', 'left-start', 'left-end'. |
| triggers | string[] | ['click'] | Events that show the popper: 'click', 'hover', 'focus', 'touch'. Empty array means manual control via v-model. |
| popper-hide-triggers | string[] | ['click'] | Events that hide the popper. Same values as triggers. |
| auto-hide | boolean | true | When true, hides the popper when clicking outside. |
| delay | number | 0 | Delay in milliseconds before showing/hiding. |
| distance | number \| string | "4" | Distance in pixels between the popper and reference element. |

### Slots

| Name | Description |
|------|-------------|
| default | The trigger element that shows/hides the popper content. |
| content | The content displayed in the popper when triggered. |

---

## Snackbar

A toast notification component for displaying messages and performing actions, using Teleport to attach to the document body.

### Basic Usage

```vue
<template>
  <spr-snackbar ref="snackbar" />

  <spr-button @click="showSnackbar1">Show Snackbar</spr-button>
</template>

<script lang="ts" setup>
import { ref } from 'vue';
const snackbar = ref(null);

const showSnackbar1 = () => {
  snackbar.value.showSnackbar({
    text: 'This is a sample message.',
  });
};
</script>
```

### Props

These properties are passed inside the payload object when calling exposed methods.

| Name | Type | Default | Description |
|------|------|---------|-------------|
| text | string | **Required** | Text message displayed in the snackbar. |
| tone | 'success' \| 'information' \| 'danger' \| 'caution' | 'information' | Color scheme and icon: success (green), information (blue), danger (red), caution (orange). |
| showIcon | boolean | false | Controls visibility of the tone-specific icon. |
| actionText | string | 'action' | Label text for the action button. |
| showAction | boolean | false | Controls visibility of the action button/text. |
| action | Function | () => {} | Function executed when action is clicked. Default closes the snackbar. |
| duration | number | 4000 | Duration in milliseconds before auto-disappearing. |
| actionIconProps | { icon: string; tone: 'neutral' \| 'success' \| 'danger' } | undefined | Configuration for an icon-only action button. |

### Events

| Name | Parameters | Description |
|------|------------|-------------|
| click | (evt: MouseEvent) | Emitted when the snackbar is clicked. |

### Slots

| Name | Description |
|------|-------------|
| snackbarActions | Custom action section content. `showAction` must be true for this slot to render. |
| icon | Custom icon content for the snackbar. |
| label | Custom text label content for the snackbar. |

### Exposed Methods

| Name | Parameters | Description |
|------|------------|-------------|
| showSnackbar | (payload: SnackPropTypes) | Display a snackbar with full configuration. |
| showSuccess | (payload: SnackPropTypes) | Display a success (green) snackbar. Tone is auto-set. |
| showInformation | (payload: SnackPropTypes) | Display an information (blue) snackbar. Tone is auto-set. |
| showDanger | (payload: SnackPropTypes) | Display a danger (red) snackbar. Tone is auto-set. |
| showCaution | (payload: SnackPropTypes) | Display a caution (orange) snackbar. Tone is auto-set. |

---

## Sidepanel

A slide-in panel from the edge of the screen for supplementary content, forms, or detailed information.

### Basic Usage

```vue
<template>
  <spr-button tone="success" @click="isSidepanelOpen = true">Open Sidepanel</spr-button>

  <spr-sidepanel :is-open="isSidepanelOpen" @close="isSidepanelOpen = false" header-title="Sidepanel Example">
    <div class="spr-p-4">Sidepanel Content</div>

    <template #footer>
      <div class="spr-flex spr-justify-end spr-gap-2 spr-px-4">
        <spr-button @click="isSidepanelOpen = false">Cancel</spr-button>
        <spr-button tone="success">Submit</spr-button>
      </div>
    </template>
  </spr-sidepanel>
</template>

<script setup>
import { ref } from 'vue';

const isSidepanelOpen = ref(false);
</script>
```

### Props

| Name | Type | Default | Description |
|------|------|---------|-------------|
| is-open | boolean | false | Controls whether the sidepanel is open or closed. |
| header-title | string | 'Sidepanel Header' | Title text displayed in the header. |
| headerSubtitle | string | - | Subtitle text displayed under the header title. |
| size | 'sm' \| 'md' \| 'lg' | 'sm' | Width of the sidepanel: sm (360px), md (420px), lg (480px). |
| height | string \| number | 'calc(100vh - 32px)' | Height of the sidepanel. Number is treated as pixels; string accepts CSS units. |
| hide-header | boolean | false | When true, hides the default header section. |
| position | 'right' | 'right' | Side the panel appears from. |
| has-backdrop | boolean | true | Displays a semi-transparent backdrop behind the sidepanel. |
| close-outside | boolean | true | Allows closing by clicking outside the panel. |
| escape-close | boolean | true | Allows closing by pressing the ESC key. |
| is-stacking | boolean | false | Enables stacking behavior for nested panels. |
| footer-no-padding | boolean | false | Removes default padding from the footer section. |
| footerNoTopBorder | boolean | false | Removes the top border from the footer. |
| isExpandable | boolean | false | Renders an expand/shrink icon in the header. |
| isExpanded | boolean | false | Whether the sidepanel is currently expanded. |
| isActivePanel | boolean | false | Whether this panel is the active one in a stacked configuration. |
| isLoading | boolean | false | When true, renders skeleton loaders in the header. |

### Events

| Name | Parameters | Description |
|------|------------|-------------|
| close | None | Emitted when the sidepanel should close (close button, outside click, or ESC). |
| on-close | None | Emitted after the sidepanel has been closed. |
| expand | None | Emitted when the expand icon is clicked. |
| shrink | None | Emitted when the shrink icon is clicked. |

### Slots

| Name | Description |
|------|-------------|
| default | Main content area of the sidepanel. |
| header | Custom header content. You must implement your own close button when using this slot. |
| subtitle | Custom header subtitle content. |
| footer | Footer section, typically containing action buttons. |

---

## StackingSidepanel

A container that allows multiple sidepanels to stack horizontally, creating layered navigation for multi-step workflows and master-detail views.

### Basic Usage

```vue
<template>
  <spr-button @click="stackingSidepanel?.showPanel('sidepanel-1')">Show Panel 1</spr-button>

  <spr-stacking-sidepanel ref="stacking-sidepanel" v-model:stack="activePanel" @update:stack="activePanelsHandler">
    <template #sidepanel-1>
      <spr-sidepanel
        size="sm"
        position="right"
        :is-stacking="true"
        header-title="Sidepanel 1"
        @close="stackingSidepanel?.hidePanel('sidepanel-1')"
      >
        <div class="spr-p-size-spacing-2xs">
          <spr-button @click="stackingSidepanel?.showPanel('sidepanel-2')">Show Panel 2</spr-button>
        </div>
      </spr-sidepanel>
    </template>
    <template #sidepanel-2>
      <spr-sidepanel
        size="md"
        position="right"
        :is-stacking="true"
        header-title="Sidepanel 2"
        @close="stackingSidepanel?.hidePanel('sidepanel-2')"
      >
        <div class="spr-p-size-spacing-2xs">Panel 2 content</div>
      </spr-sidepanel>
    </template>
  </spr-stacking-sidepanel>
</template>

<script lang="ts" setup>
import { ref, useTemplateRef } from 'vue';

const activePanel = ref<string[]>([]);
const stackingSidepanel = useTemplateRef("stacking-sidepanel");

const activePanelsHandler = (panel: string[]) => {
  console.log('Active panels:', panel);
};
</script>
```

### Props

| Name | Type | Default | Description |
|------|------|---------|-------------|
| stack | string[] | [] | Array of active panel names. Used with `v-model:stack` for two-way binding. |

### Events

| Name | Parameters | Description |
|------|------------|-------------|
| update:stack | (panels: string[]) | Emitted when the stack of panels changes. |

### Exposed Methods

| Name | Parameters | Description |
|------|------------|-------------|
| showPanel | (name: string) | Shows a panel by its slot name. |
| hidePanel | (name: string) | Hides a panel by its slot name. |
| handleExpandPanel | (action: 'expand' \| 'shrink', name: string) | Expands or shrinks a panel. |

### Exposed Variables

| Name | Description |
|------|-------------|
| expandedPanel | Name of the currently expanded sidepanel. |
| activePanel | Name of the currently shown sidepanel. |

### Slots

| Name | Description |
|------|-------------|
| (named slots) | Each panel is rendered in a named slot using `<template #panelName>`. Slot names must be unique. Each slot should contain an `spr-sidepanel` with `is-stacking` set to true. |

---

## FloatingAction

A fixed action bar that appears at the bottom of the screen with a smooth slide-up animation for contextual actions.

### Basic Usage

```vue
<template>
  <spr-button size="large" tone="success" @click="showFloatingAction = true">Show Floating Action</spr-button>

  <spr-floating-action :show="showFloatingAction">
    <template #message>
      <div class="spr-flex spr-items-center spr-gap-1">
        <Icon class="spr-size-5 spr-text-mango-500" icon="ph:warning-fill" />
        <span>You have unsaved changes</span>
      </div>
    </template>
    <template #actions>
      <spr-button size="large" variant="secondary" @click="showFloatingAction = false">Discard</spr-button>
      <spr-button size="large" tone="success">Save Changes</spr-button>
    </template>
  </spr-floating-action>
</template>

<script setup>
import { ref } from 'vue';

const showFloatingAction = ref(false);
</script>
```

### Props

| Name | Type | Default | Description |
|------|------|---------|-------------|
| show | boolean | false | Controls visibility. When true, the component slides up into view; when false, it slides down and hides. |

### Slots

| Name | Description |
|------|-------------|
| message | Left-side content for notification text, warnings, or informational messages. |
| actions | Right-side content for buttons or interactive elements. |
| default | When used, replaces the predefined message/actions layout for complete customization. |
