# Display Components Reference

## Table of Contents

- [Avatar](#avatar) - Visual representation of users or entities
- [Badge](#badge) - Small visual indicator for status or notifications
- [Banner](#banner) - Important messages, statuses, or alerts
- [Chips](#chips) - Interactive elements for filtering, selection, and tagging
- [Icon](#icon) - Consistent icon display with sizes, tones, and variants
- [Logo](#logo) - Sprout product logo display
- [Lozenge](#lozenge) - Status labels with icons, images, and interactive states
- [ProgressBar](#progressbar) - Visual progress indicator for tasks
- [Status](#status) - Standardized status indicators
- [EmptyState](#emptystate) - Placeholder when no content is available
- [AuditTrail](#audittrail) - Chronological log of record changes

---

## Avatar

Visual representation of users or entities with support for images, initials, icons, notifications, and badges.

### Basic Usage

```vue
<template>
  <spr-avatar />
</template>
```

### Props

| Name | Type | Default | Description |
|------|------|---------|-------------|
| variant | `'image' \| 'initial' \| 'client' \| 'user' \| 'user-group' \| 'count'` | `'initial'` | Defines the type of avatar to display. `image`: uses `src` prop; `initial`: shows text initials; `client`: building icon; `user`: single person icon; `user-group`: multiple people icon; `count`: numeric count with "+" prefix. |
| src | `string` | `undefined` | URL for the avatar image when using `variant="image"`. |
| alt | `string` | `'Avatar'` | Alternative text for the avatar image for accessibility. |
| size | `'2xl' \| 'xl' \| 'lg' \| 'md' \| 'sm' \| 'xs' \| '2xs'` | `'2xl'` | Controls the size of the avatar. 2xl=80px, xl=56px, lg=40px, md=36px, sm=24px, xs=20px, 2xs=16px. |
| notification | `boolean` | `false` | Displays a notification indicator at the top-right corner. |
| notificationText | `string` | `'0'` | Text to display in the notification indicator. |
| badge | `boolean` | `false` | Displays a status badge at the bottom-right corner. |
| initial | `string` | `'Avatar'` | Text to extract initials from when using `variant="initial"`. Single name uses first letter; multiple names use first and last initials. |
| color | `'primary' \| 'secondary'` | `'primary'` | Background color scheme. `primary`: surface color; `secondary`: standard background color. |
| status | `'brand' \| 'information' \| 'danger' \| 'disabled'` | `'brand'` | Status indicator type when `badge` is true. brand=green, information=blue, danger=red, disabled=gray. |
| count | `number` | `0` | Numeric value displayed with "+" prefix when using `variant="count"`. |
| loading | `boolean` | `false` | Displays a skeletal loading state animation. |

### Events

| Name | Parameters | Description |
|------|-----------|-------------|
| imageError | - | Emitted when the image source fails to load. |

### Slots

| Name | Description |
|------|-------------|
| default | Custom content to display inside the avatar. Takes precedence over standard icon or initial display. Adjust content to fit avatar size. |

---

## Badge

Small visual indicator for conveying information, status, or notifications, attachable to other elements.

### Basic Usage

```vue
<template>
  <spr-badge text="9" variant="brand" size="big" />
</template>
```

### Props

| Name | Type | Default | Description |
|------|------|---------|-------------|
| text | `string` | `'0'` | Content displayed inside the badge. Not displayed when size is `tiny`. |
| variant | `'brand' \| 'information' \| 'danger' \| 'disabled' \| 'neutral'` | `'brand'` | Color scheme of the badge. brand=primary color, information=blue, danger=red, disabled=gray. |
| size | `'big' \| 'small' \| 'tiny'` | `'small'` | Size of the badge. big=20px, small=16px, tiny=10px (dot only). |
| position | `'top' \| 'bottom' \| 'default'` | `'default'` | Position relative to slotted content. `top`/`bottom` position at corners; `default` renders standalone. |

### Slots

| Name | Description |
|------|-------------|
| default | Content to which the badge will be attached. Required when using `top` or `bottom` position. |

---

## Banner

Displays important messages, statuses, or alerts with support for different types and dismissibility.

### Basic Usage

```vue
<template>
  <spr-banner v-model:show="showBanner" type="success" message="This is a success banner." />
</template>

<script setup lang="ts">
import { ref } from 'vue';
const showBanner = ref(true);
</script>
```

### Props

| Name | Type | Default | Description |
|------|------|---------|-------------|
| type | `'success' \| 'error' \| 'info' \| 'pending' \| 'caution'` | `'success'` | Visual appearance and semantic meaning. Each type uses specific colors and icons. |
| showCloseButton | `boolean` | `false` | Displays a close button to dismiss the banner. |
| message | `string` | `undefined` | Text content displayed in the banner. Overridden by default slot content if provided. |
| show (v-model) | `boolean` | `true` | Controls visibility. Use `v-model:show` for two-way binding. Banner is removed from DOM when hidden. |

### Events

| Name | Parameters | Description |
|------|-----------|-------------|
| update:show | `(value: boolean)` | Emitted when visibility changes via close button or programmatic changes. Used for `v-model:show`. |

### Slots

| Name | Description |
|------|-------------|
| default | Custom content replacing the standard message display. Takes precedence over the `message` prop. |

---

## Chips

Interactive compact elements for filtering, selection, tagging, and displaying small pieces of information.

### Basic Usage

```vue
<template>
  <spr-chips label="Basic Chip" />
  <spr-chips label="Active Chip" :active="true" />
</template>
```

### Props

| Name | Type | Default | Description |
|------|------|---------|-------------|
| label | `string` | `''` | Text content displayed in the chip. |
| size | `'sm' \| 'md' \| 'lg'` | `'md'` | Controls the size of the chip. |
| tone | `'subtle' \| 'default'` | `'default'` | Controls the tone of the chip. |
| active | `boolean` | `false` | Determines if the chip is in an active/selected state. |
| disabled | `boolean` | `false` | Makes the chip non-interactive with disabled styling. |
| closable | `boolean` | `false` | Displays a close button that emits a close event when clicked. |
| variant | `'tag' \| 'day'` | `'tag'` | Changes appearance and behavior. `day` variant shows circular day-of-week chips. |
| icon | `string` | `''` | Iconify icon name to display before the label. |
| iconWeight | `'regular' \| 'bold' \| 'thin' \| 'light' \| 'fill' \| 'duotone'` | `'regular'` | Visual weight/style of the icon. |
| closeIconSize | `number` | `16` | Size of the close icon in pixels. |
| avatarUrl | `string` | `''` | URL of the image to display in the avatar. |
| avatarVariant | `'image' \| 'text' \| 'client' \| 'user'` | `''` | Type of avatar to display. |
| avatarInitials | `string` | `''` | Text initials when avatarVariant is `'text'`. |
| badge | `boolean` | `false` | Displays a badge on the chip. |
| badgeText | `string` | `'0'` | Text content of the badge. |
| badgeVariant | `'brand' \| 'danger' \| 'disabled'` | `'brand'` | Visual style of the badge. |
| visible | `boolean` | `true` | Controls whether the chip is rendered. |
| day | `'Sunday' \| 'Monday' \| 'Tuesday' \| 'Wednesday' \| 'Thursday' \| 'Friday' \| 'Saturday'` | - | Day name for the day variant. |

### Events

| Name | Parameters | Description |
|------|-----------|-------------|
| update | `(value: boolean)` | Emitted when the chip's active state changes. |
| close | `(event: MouseEvent \| KeyboardEvent)` | Emitted when the close button is clicked (when closable is true). |

### Slots

| Name | Description |
|------|-------------|
| default | Custom content that completely overrides the standard chip display. |
| icon | Custom content for the icon area (when using standard chip structure). |
| close-icon | Custom content for the close button icon (when closable is true). |

---

## Icon

Consistent icon display with various sizes, semantic tones, and visual variants using Iconify integration.

### Basic Usage

```vue
<template>
  <spr-icon id="basic-icon" icon="ph:user" />
</template>
```

### Props

| Name | Type | Default | Description |
|------|------|---------|-------------|
| id | `string` | - | Unique identifier for the icon. Required. |
| icon | `string` | `''` | Iconify icon name (e.g., `'ph:user'`, `'ph:check-circle'`). |
| size | `'2xs' \| 'xs' \| 'sm' \| 'md' \| 'lg' \| 'xl' \| '2xl'` | `'md'` | Size of the icon. 2xs=16px, xs=20px, sm=24px, md=36px, lg=40px, xl=56px, 2xl=80px. |
| tone | `'success' \| 'error' \| 'info' \| 'pending' \| 'caution'` | - | Color tone. success=green, error=red, info=blue, pending=yellow, caution=orange. |
| variant | `'primary' \| 'secondary' \| 'tertiary'` | - | Visual style. primary=filled background, secondary=outlined with light background, tertiary=plain without background. |

---

## Logo

Displays Sprout product logos with configurable theme and sizing.

### Basic Usage

```vue
<template>
  <spr-logo />
</template>
```

### Props

| Name | Type | Default | Description |
|------|------|---------|-------------|
| name | `string` | `'hr'` | Product logo to display. Available: `benchmark`, `ecosystem`, `engage`, `finances`, `hr-mobile`, `hr`, `inbound`, `insight`, `readycash`, `readywage`, `payroll`, `performance-plus`, `procurement`, `professional-services`, `recruit`, `recruit-plus`, `sail`, `sidekick`, `wellness`. |
| theme | `string` | `'dark'` | Color theme. Available: `white` (dark backgrounds), `dark` (light backgrounds), `gray` (subtle), `green` (brand emphasis). |
| width | `string \| number` | `50` | Width of the logo. Number is interpreted as pixels; string accepts CSS units (e.g., `'5em'`). |

---

## Lozenge

Status labels with support for icons, images, avatars, interactive states, and loading indicators.

### Basic Usage

```vue
<template>
  <spr-lozenge label="Lozenge" />
</template>
```

### Props

| Name | Type | Default | Description |
|------|------|---------|-------------|
| label | `string` | `'label'` | Primary text content displayed in the lozenge. |
| tone | `'plain' \| 'pending' \| 'information' \| 'success' \| 'neutral' \| 'caution' \| 'danger'` | `'plain'` | Color scheme indicating status or categorization. |
| fill | `boolean` | `false` | When true, solid background. When false, outline style with transparent background. |
| removable | `boolean` | `false` | Shows a remove icon allowing the user to remove the lozenge. |
| url | `string` | `''` | URL for the avatar image displayed within the lozenge. |
| visible | `boolean` | `true` | Controls visibility. When false, the lozenge is not rendered. |
| loading | `boolean` | `false` | Displays a skeletal loading state. |
| icon | `string` | `''` | Iconify icon name displayed as a prefix icon before the label. |
| postfix-icon | `string` | `''` | Iconify icon name displayed after the label. |
| interactive | `boolean` | `false` | Enables hover and active states for clickable behavior. |
| dropdown | `boolean` | `false` | Behaves as a dropdown trigger with a caret icon. Automatically sets interactive to true. |
| max-width | `string` | `'none'` | Maximum width for the lozenge. Text beyond this width is truncated with ellipsis. Accepts CSS width values. |

### Events

| Name | Parameters | Description |
|------|-----------|-------------|
| onRemove | `(event: MouseEvent)` | Emitted when the remove button is clicked on a removable lozenge. |

### Slots

| Name | Description |
|------|-------------|
| icon | Custom content for the prefix icon area (before the label). |
| avatar | Custom content for the avatar area. |
| postfixIcon | Custom content for the postfix icon area (after the label). |

---

## ProgressBar

Visually represents progress of a task with support for sizes, colors, labels, and custom max values.

### Basic Usage

```vue
<template>
  <spr-progress-bar :value="25" />
</template>
```

### Props

| Name | Type | Default | Description |
|------|------|---------|-------------|
| value | `number` | `0` | Current progress value. Calculated as percentage of `max`. |
| size | `'xs' \| 'sm' \| 'lg'` | `'lg'` | Height of the progress bar. xs=4px, sm=8px, lg=12px. |
| max | `number` | `100` | Maximum value (1-100). Progress is calculated as percentage of this. |
| label | `boolean` | `true` | Displays a percentage label. |
| color | `'success' \| 'danger' \| 'warning' \| 'info' \| 'neutral'` | `'success'` | Color theme. success=green, danger=red, warning=orange, info=blue, neutral=gray. |
| label-placement | `'top' \| 'top-start' \| 'top-center' \| 'top-end' \| 'bottom' \| 'bottom-start' \| 'bottom-center' \| 'bottom-end' \| 'left' \| 'right'` | `'bottom'` | Position and alignment of the percentage label relative to the bar. |
| supporting-label | `string` | `''` | Additional text alongside the percentage label for context (e.g., "of 100 MB"). |

---

## Status

Standardized status indicators with consistent color, iconography, and sizing.

### Basic Usage

```vue
<template>
  <spr-status state="success" />
</template>
```

### Props

| Name | Type | Default | Description |
|------|------|---------|-------------|
| state | `'success' \| 'information' \| 'pending' \| 'caution' \| 'danger'` | `'success'` | Status state. success=green check circle, information=blue info, pending=yellow warning, caution=orange warning, danger=red warning circle. |
| size | `'2xs' \| 'xs' \| 'sm' \| 'base' \| 'lg' \| 'xl' \| '2xl'` | `'base'` | Size of the indicator. 2xs=14px, xs=16px, sm=20px, base=24px, lg=32px, xl=40px, 2xl=48px. |

---

## EmptyState

Informs users when no content is available, with optional imagery and action buttons.

### Basic Usage

```vue
<template>
  <spr-empty-state description="No results found" subDescription="Try a different search term">
    <template #button>
      <spr-button tone="success">Retry</spr-button>
    </template>
  </spr-empty-state>
</template>
```

### Props

| Name | Type | Default | Description |
|------|------|---------|-------------|
| description | `string` | `'No results found'` | Main description text displayed prominently. |
| subDescription | `string` | `'Try a different search term.'` | Secondary text providing additional context below the description. |
| size | `'small' \| 'large'` | `'small'` | Overall size. small: min-height 240px, image 120x120px. large: min-height 360px, image 200x200px. |
| image | `string` | `'list'` | Predefined image name. Available: `bug`, `clock`, `dashboard`, `employees`, `government-id`, `integration`, `list`, `social-media-handles`, `work-in-progress`, `work-location`. |
| hasButton | `boolean` | `false` | Indicates whether the empty state includes a button. |
| emptyStateCustomClasses | `string` | `''` | Additional CSS classes for the container. |

### Events

| Name | Parameters | Description |
|------|-----------|-------------|
| onClick | None | Emitted when the component is clicked. |

### Slots

| Name | Description |
|------|-------------|
| default | Replaces the predefined image with custom content. Receives size classes based on `size` prop. |
| button | Action buttons or interactive elements displayed below the description. |

---

## AuditTrail

Displays a chronological log of changes made to a record with expandable entries.

### Basic Usage

```vue
<template>
  <SprAuditTrail :auditTrailLogs="logs" />
</template>

<script setup lang="ts">
const logs = [
  {
    userName: 'John Doe',
    title: 'John Doe UPDATED this on October 22, 2025 at 10:30 AM',
    logs: [
      {
        label: ['Status'],
        oldValue: 'Inactive',
        newValue: 'Active',
      },
    ],
  },
];
</script>
```

### Props

| Name | Type | Default | Description |
|------|------|---------|-------------|
| auditTrailLogs | `{ userName: string; title: string; avatarUrl?: string; logs: { label: string[]; oldValue: string; newValue: string; }[]; }[]` | `[]` | List of audit trail log entries. If avatarUrl is not provided, a default avatar with initials based on userName is rendered. |
| alwaysOpen | `boolean` | `true` | When true, log entries remain opened when opening another entry. |
