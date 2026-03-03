# Action, Picker & Filter Components Reference

## Table of Contents

- [Button](#button)
- [ButtonDropdown](#buttondropdown)
- [DatePicker](#datepicker)
- [DateCalendarPicker](#datecalendarpicker)
- [DateRangePicker](#daterangepicker)
- [MonthYearPicker](#monthyearpicker)
- [TimePicker](#timepicker)
- [Calendar](#calendar)
- [CalendarCell](#calendarcell)
- [Filter](#filter)
- [AttributeFilter](#attributefilter)

---

## Button

A versatile button component for triggering actions, with customizable size, tone, variant, and icon support.

### Basic Usage

```vue
<template>
  <spr-button tone="success" variant="secondary" size="large" hasIcon>
    <Icon icon="ph:trash" />
    <span>Button</span>
  </spr-button>
</template>

<script lang="ts" setup>
import { Icon } from '@iconify/vue';
</script>
```

### Props

| Name | Type | Default | Description |
|------|------|---------|-------------|
| tone | `'neutral' \| 'success' \| 'danger'` | `'neutral'` | Controls the button's color theme. |
| size | `'small' \| 'medium' \| 'large'` | `'medium'` | Defines the button's size, affecting padding, font size, and overall dimensions. |
| variant | `'primary' \| 'secondary' \| 'tertiary'` | `'primary'` | Controls the button's visual style. `primary` has the strongest emphasis, `secondary` has an outline, `tertiary` is the subtlest. |
| type | `'button' \| 'submit' \| 'reset'` | `'button'` | Specifies the native HTML button type attribute. |
| state | `'base' \| 'hover' \| 'pressed' \| 'focus'` | `'base'` | Defines the visual state of the button. Mostly used internally. |
| disabled | `boolean` | `false` | When true, prevents user interaction and applies a visual disabled state. |
| hasIcon | `boolean` | `false` | Indicates that the button contains an icon, which affects spacing and layout. |
| fullwidth | `boolean` | `false` | When true, the button expands to fill the width of its container. |

### Events

| Name | Parameters | Description |
|------|------------|-------------|
| click | `(event: MouseEvent)` | Emitted when the button is clicked and not disabled. |

### Slots

| Name | Description |
|------|-------------|
| default | Content to be displayed inside the button. This can include text, icons, or other elements. |

---

## ButtonDropdown

Combines a primary action button with a dropdown menu for additional options, supporting tones, sizes, and variants.

### Basic Usage

```vue
<template>
  <spr-button-dropdown
    dropdown-id="example1"
    v-model="selected"
    :menu-list="menuList"
    width="160px"
    popper-width="160px"
    @click="mainButtonClicked"
  >
    Button Dropdown
  </spr-button-dropdown>
</template>

<script setup lang="ts">
import { ref } from 'vue';
const selected = ref([]);
const menuList = ref([
  {
    text: 'Add',
    value: 'add',
    icon: 'ph:check',
    iconColor: 'spr-text-color-supporting',
    onClickFn: () => { alert('Add was clicked.'); },
  },
  {
    text: 'Delete',
    value: 'delete',
    icon: 'ph:trash',
    textColor: 'spr-text-color-danger-base',
    onClickFn: () => { alert('Delete was clicked.'); },
  },
]);

const mainButtonClicked = () => { alert('Main button was clicked.'); };
</script>
```

### Props

| Name | Type | Default | Description |
|------|------|---------|-------------|
| modelValue | `MenuListType[] \| string[] \| Record<string, unknown>[]` | `[]` | Currently selected menu item(s). Used with `v-model` for two-way binding. |
| menuList | `MenuListType[] \| string[] \| Record<string, unknown>[]` | `[]` | Array of options to display in the dropdown. Each item can include `text`, `value`, `icon`, `iconColor`, `textColor`, `onClickFn`, `disabled`. |
| dropdownId | `string` | Required | Required unique identifier for the dropdown component. |
| tone | `'neutral' \| 'success'` | `'neutral'` | Controls the color theme. Note: `danger` tone is not available for button-dropdown. |
| variant | `'primary' \| 'secondary'` | `'primary'` | Controls the visual style. Note: `tertiary` variant is not available. |
| size | `'small' \| 'medium' \| 'large'` | `'medium'` | Controls the size of the button dropdown. |
| disabled | `boolean` | `false` | When true, both the main button and dropdown trigger become non-interactive. |
| width | `string` | `'fit-content'` | Sets the width of the entire button dropdown component. |
| popperWidth | `string` | `'fit-content'` | Sets the width of the dropdown menu container. |
| popperInnerWidth | `string` | `'unset'` | Sets the width of the content inside the dropdown menu. |
| placement | `'auto' \| 'auto-start' \| 'auto-end' \| 'top' \| 'top-start' \| 'top-end' \| 'right' \| 'right-start' \| 'right-end' \| 'bottom' \| 'bottom-start' \| 'bottom-end' \| 'left' \| 'left-start' \| 'left-end'` | `'bottom'` | Controls the position of the dropdown menu relative to the button. |

### Events

| Name | Parameters | Description |
|------|------------|-------------|
| click | `MouseEvent` | Emitted when the main (left) button is clicked. |
| update:modelValue | `MenuListType[] \| string[] \| Record<string, unknown>[]` | Emitted when a selection is made in the dropdown menu. |

### Slots

| Name | Description |
|------|-------------|
| default | Content for the main (left) button. The dropdown button (right) always displays a caret-down icon and cannot be customized. |

### MenuListType Interface

```typescript
type MenuListType = {
  text: string;
  value: string | number;
  icon?: string;
  iconColor?: string;
  textColor?: string;
  onClickFn?: () => void;
  disabled?: boolean;
  subtext?: string;
  subvalue?: string;
  sublevel?: MenuListType[];
  group?: string;
};
```

---

## DatePicker

Allows users to select a date from a calendar interface, supporting various formats, disabled dates, and customization options.

### Basic Usage

```vue
<template>
  <spr-date-picker id="datepicker" v-model="datePickerModel" label="Date Picker" format="MM-DD-YYYY" />
</template>

<script lang="ts" setup>
import { ref } from 'vue';

const datePickerModel = ref('');
</script>
```

### Props

| Name | Type | Default | Description |
|------|------|---------|-------------|
| id | `String` | - | Required to bind popper within the datepicker. |
| v-model | `String` | - | Binds the selected date value. |
| label | `String` | - | Label text for the input field. |
| width | `String` | `'400px'` | Sets the width of the input. |
| format | `String` | `'MM-DD-YYYY'` | Format for the selected date (e.g., `'MM-DD-YYYY'`, `'YYYY-MM-DD'`, `'MM/DD/YYYY'`). |
| disabled | `Boolean` | `false` | Disables the date picker. |
| readonly | `Boolean` | `false` | Makes the date picker read-only. |
| readonly2 | `Boolean` | `false` | Alternative read-only mode with compact styling (no padding, smaller font). Disables calendar popup and input editing. |
| helper-text | `String` | - | Displays a helper message below the input. |
| helper-icon | `String` | - | Icon to display alongside the helper message. |
| display-helper | `Boolean` | `false` | Shows the helper message. |
| error | `Boolean` | `false` | Indicates that there is an error with the input. |
| current-year | `String` | Current year | Sets the current year in the calendar view. |
| min-max-year | `Object` | `{ min: 1900, max: currentYear }` | Sets the minimum and maximum years in the calendar. |
| rest-days | `Array` | `[]` | Array of days to be treated as rest days (e.g., `['su', 'mo', 'tu', 'we', 'th', 'fr', 'sa']`). |
| disabled-dates | `Object` | `{}` | Disables specific dates or date ranges. Supports `from`, `to`, `pastDates`, `futureDates`, `selectedDates`, `weekends`, `weekdays`, `selectedDays`. |
| placement | `String` | `'bottom'` | Changes the placement of the dropdown popper. |
| wrapper-position | `String` | `'relative'` | CSS position of the date picker wrapper. |
| popper-strategy | `String` | `'absolute'` | Popper positioning strategy (`'absolute'` or `'fixed'`). |
| popper-container | `String \| HTMLElement` | `''` | CSS selector or HTMLElement for a custom popper container. |

### Events

| Name | Parameters | Description |
|------|------------|-------------|
| update:model-value | `String` (formatted date) | Emits when the selected date value changes. |
| get-input-value | `String \| null` | Emits the actual date being typed or selected. |
| get-date-formats | `Object` | Emits available date formats when a valid date is selected. |
| get-month-list | `Array` | Emits the list of months available in the component. |
| get-year-list | `Array` | Emits the list of years available in the component. |
| get-date-errors | `Array` | Emits validation errors from the date picker. |

---

## DateCalendarPicker

A standalone calendar picker with v-model support, multiple modes (full, month-year, year-only), and comprehensive date restriction options.

### Basic Usage

```vue
<template>
  <spr-date-calendar-picker
    v-model="selectedDate"
    :min-max-year="{ min: 2024, max: 2025 }"
    @update:month="handleMonthUpdate"
    @update:year="handleYearUpdate"
    @update:day="handleDayUpdate"
  />
</template>

<script setup>
import { ref } from 'vue';

const selectedDate = ref('2024-01-15');

const handleMonthUpdate = (month) => { console.log('Month updated:', month); };
const handleYearUpdate = (year) => { console.log('Year updated:', year); };
const handleDayUpdate = (day) => { console.log('Day updated:', day); };
</script>
```

### Props

| Name | Type | Default | Description |
|------|------|---------|-------------|
| modelValue | `string` | `''` | The selected date value (v-model). |
| selectedMonth | `number` | `undefined` | Pre-selected month (0-indexed). |
| selectedYear | `number` | `undefined` | Pre-selected year. |
| selectedDay | `number` | `undefined` | Pre-selected day. |
| minMaxYear | `MinMaxYearType` | `{ min: 1900, max: currentYear }` | Year range constraints. |
| restDays | `RestDayType[]` | `[]` | Days to mark as rest days (`'su' \| 'mo' \| 'tu' \| 'we' \| 'th' \| 'fr' \| 'sa'`). |
| disabledDates | `DisabledDatesType` | `undefined` | Date restrictions (supports `from`, `to`, `pastDates`, `futureDates`, `selectedDates`, `weekends`, `weekdays`, `selectedDays`). |
| disabled | `boolean` | `false` | Disable the calendar. |
| readonly | `boolean` | `false` | Make the calendar read-only. |
| mode | `'full' \| 'month-year' \| 'year-only'` | `'full'` | Calendar display mode. |
| format | `string` | `'MM-DD-YYYY'` | Date format for v-model. |

### Events

| Name | Parameters | Description |
|------|------------|-------------|
| update:modelValue | `(value: string)` | Emitted when the selected date changes. |
| update:month | `(month: number)` | Emitted when month changes (0-indexed). |
| update:year | `(year: number)` | Emitted when year changes. |
| update:day | `(day: number)` | Emitted when day changes. |

---

## DateRangePicker

Enables users to select a start and end date from a calendar interface, with options for customization, disabled dates, and various display formats.

### Basic Usage

```vue
<template>
  <spr-date-range-picker id="date-range-basic" v-model="dateRangeModel" display-helper />
</template>

<script lang="ts" setup>
import { ref } from 'vue';
const dateRangeModel = ref({ startDate: '', endDate: '' });
</script>
```

### Props

| Name | Type | Default | Description |
|------|------|---------|-------------|
| id | `String` | - | Required to bind popper within the calendar. |
| v-model | `Object` | `{ startDate: '', endDate: '' }` | Binds the selected date range value. |
| label | `String` | - | Label text for the input field. |
| width | `String` | `'400px'` | Sets the width of the input. |
| format | `String` | `'MM-DD-YYYY'` | Format for the selected date. |
| disabled | `Boolean` | `false` | Disables the date range calendar. |
| readonly | `Boolean` | `false` | Makes the date range calendar read-only. |
| helper-text | `String` | - | Displays a helper message below the input. |
| helper-icon | `String` | - | Icon to display alongside the helper message. |
| display-helper | `Boolean` | `false` | Shows the helper message. |
| error | `Boolean` | `false` | Indicates that there is an error with the input. |
| current-year | `String` | Current year | Sets the current year in the calendar view. |
| min-max-year | `Object` | `{ min: 1900, max: currentYear }` | Sets the minimum and maximum years in the calendar. |
| rest-days | `Array` | `[]` | Array of days to be treated as rest days. |
| disabled-dates | `Object` | `{}` | Disables specific dates or date ranges. |
| placement | `String` | `'bottom'` | Changes the placement of the dropdown popper. For default inputs, concatenates with `-start` or `-end` based on the clicked field. |
| wrapper-position | `String` | `'relative'` | CSS position of the date range picker wrapper. |
| popper-strategy | `String` | `'absolute'` | Popper positioning strategy. |
| popper-container | `String \| HTMLElement` | `''` | CSS selector or HTMLElement for a custom popper container. |

### Events

| Name | Parameters | Description |
|------|------------|-------------|
| update:model-value | `Object` | Emits when the selected date range changes. |
| get-input-value | `Object` | Emits the actual date range being typed or selected. |
| get-date-formats | `Object` | Emits available date formats for the selected range. |
| get-month-list | `Array` | Emits the list of months. |
| get-year-list | `Array` | Emits the list of years. |
| get-date-errors | `Array` | Emits the available date errors. |
| range-change | `Object` | Emits when the range changes. |

### Slots

| Name | Description |
|------|-------------|
| default | Custom input area. Receives `{ handleClick }` slot prop to open the calendar. |
| helperMessage | Custom helper message content below the input. |

---

## MonthYearPicker

Allows users to select a month and year from a popup interface, supporting various formats and customization options.

### Basic Usage

```vue
<template>
  <spr-month-year-picker id="monthyearpicker" v-model="monthYearPickerModel" label="Select Month and Year" display-helper />
</template>

<script lang="ts" setup>
import { ref } from 'vue';

const monthYearPickerModel = ref('');
</script>
```

### Props

| Name | Type | Default | Description |
|------|------|---------|-------------|
| id | `String` | - | Required to bind popper within the month year picker. |
| v-model | `String` | - | Binds the selected month-year value. |
| label | `String` | - | Label text for the input field. |
| width | `String` | `'100%'` | Sets the width of the input. |
| format | `String` | `'MM-YYYY'` | Format for the selected month-year (e.g., `'MM-YYYY'`, `'YYYY-MM'`, `'MMMM YYYY'`). |
| disabled | `Boolean` | `false` | Disables the month year picker. |
| readonly | `Boolean` | `false` | Makes the month year picker read-only. |
| helper-text | `String` | - | Displays a helper message below the input. |
| helper-icon | `String` | - | Icon to display alongside the helper message. |
| display-helper | `Boolean` | `false` | Shows the helper message. |
| error | `Boolean` | `false` | Indicates that there is an error with the input. |
| current-year | `String` | Current year | Sets the current year in the picker view. |
| min-max-year | `Object` | `{ min: 1900, max: currentYear }` | Sets the minimum and maximum years in the picker. |
| placement | `String` | `'bottom'` | Changes the placement of the dropdown popper. |
| wrapper-position | `String` | `'relative'` | CSS position of the month year picker wrapper. |
| popper-strategy | `String` | `'absolute'` | Popper positioning strategy (`'absolute'` or `'fixed'`). |
| popper-container | `String \| HTMLElement` | `''` | CSS selector or HTMLElement for a custom popper container. |

### Events

| Name | Parameters | Description |
|------|------------|-------------|
| update:model-value | `String` (formatted month-year) | Emits when the selected month-year value changes. |
| get-input-value | `String \| null` | Emits the actual month-year being typed or selected. |
| get-date-formats | `Object` | Emits available date formats when a valid month-year is selected. |
| get-month-list | `Array` | Emits the list of months available in the component. |
| get-year-list | `Array` | Emits the list of years available in the component. |
| get-date-errors | `Array` | Emits validation errors from the month year picker. |

---

## TimePicker

Provides an interface for selecting a time from a dropdown, supporting 12-hour and 24-hour formats with customizable intervals.

### Basic Usage

```vue
<template>
  <spr-time-picker v-model="selectedValue" label="Select a time" id="time-basic" />
</template>

<script setup>
import { ref } from 'vue';

const selectedValue = ref('');
</script>
```

### Props

| Name | Type | Default | Description |
|------|------|---------|-------------|
| modelValue | `string` | Required | The selected time value (used with `v-model`). |
| format | `'12' \| '24'` | `'24'` | Time format to display. |
| interval | `number` | `30` | Time interval in minutes between options. |
| label | `string` | `''` | Label text displayed above the input field. |
| placeholder | `string` | Format-dependent (`'HH : MM'` or `'HH : MM AM/PM'`) | Placeholder text for the input field. |
| disableTyping | `boolean` | `false` | When true, prevents manual text entry in the input field. |
| error | `boolean` | `false` | When true, displays the field in an error state. |
| helperText | `string` | `''` | Helper text displayed below the input field. |
| disabled | `boolean` | `false` | When true, disables the time picker. |
| fullWidth | `boolean` | `false` | When true, the time picker expands to the full width of its container. |
| id | `string` | `'time-picker'` | HTML ID attribute for the input element. |

### Events

| Name | Parameters | Description |
|------|------------|-------------|
| update:modelValue | `string` | Emitted when the selected time changes. |

---

## Calendar

A comprehensive weekly calendar component for employee scheduling, with support for navigation, filtering, infinite scroll, and shift display.

### Basic Usage

```vue
<template>
  <SprCalendar
    v-model:search="searchEmployee"
    v-model:selected-cell="selectedCell"
    v-model:selected-company="selectedCompany"
    v-model:selected-department="selectedDepartment"
    v-model:selected-branch="selectedBranch"
    :employees="employees"
    :initial-date="initialDate"
    :company-options="companyOptions"
    :department-options="departmentOptions"
    :branch-options="branchOptions"
  />
</template>

<script setup lang="ts">
import { ref } from 'vue';

const initialDate = new Date();
const searchEmployee = ref('');
const selectedCompany = ref('');
const selectedDepartment = ref('');
const selectedBranch = ref('');
const selectedCell = ref({ employeeId: '', date: '', schedule: null });

const employees = [
  {
    id: 1,
    name: 'Theresa Webb',
    position: 'Senior UX Researcher',
    avatar: '',
    highlight: true,
    hoursWorked: 40,
    hoursTarget: 48,
    schedule: {
      '2025-05-06': [{ startTime: '09:00AM', endTime: '06:00PM', location: 'Office A', type: 'Standard Day Shift' }],
    },
  },
];

const companyOptions = [{ text: 'All Companies', value: 'all' }];
const departmentOptions = [{ text: 'All Departments', value: 'all' }];
const branchOptions = [{ text: 'All Branches', value: 'all' }];
</script>
```

### Props

| Name | Type | Default | Description |
|------|------|---------|-------------|
| employees | `Array` | `[]` | Array of employee data to display, including schedule information. |
| initialDate | `Date` | Current date | The initial date to display. The calendar shows the week containing this date. |
| loading | `boolean` | `false` | Controls whether to show a loading indicator during data fetching. |
| companyOptions | `Array<{text: string, value: string}>` | `[]` | Options for the company filter dropdown. |
| departmentOptions | `Array<{text: string, value: string}>` | `[]` | Options for the department filter dropdown. |
| branchOptions | `Array<{text: string, value: string}>` | `[]` | Options for the branch filter dropdown. |
| emptyState | `Object` | Default empty state | Configuration for the empty state when there are no employees. |

### Events

| Name | Parameters | Description |
|------|------------|-------------|
| loadMore | - | Emitted when the user scrolls near the bottom for infinite scroll. |
| onCellClick | `(data: SelectedShift)` | Emitted when a calendar cell is clicked. |
| update:firstLastDayOfWeek | `(range: { firstDay: string, lastDay: string })` | Emitted when the visible week range changes. |
| update:sort | `(value: string)` | Emitted when the sort order changes (`'asc'`, `'desc'`, or `''`). |
| update:search | `(value: string)` | Emitted when the search term changes. |
| update:selectedCell | `(data: SelectedShift)` | Emitted when a cell is selected. |
| update:selectedCompany | `(value: string)` | Emitted when the company filter selection changes. |
| update:selectedDepartment | `(value: string)` | Emitted when the department filter selection changes. |
| update:selectedBranch | `(value: string)` | Emitted when the branch filter selection changes. |
| onClickEmptyButton | - | Emitted when the empty state button is clicked. |

### Slots

| Name | Description |
|------|-------------|
| filter | Slot for customizing the filter section above the calendar. |
| loading | Slot for customizing the loading state during data fetching. |
| empty-state | Slot for customizing the empty state when there are no employees. |
| copy | Slot for customizing the copy action. Receives `{ employeeId, date, shift }`. |
| cell | Slot for customizing calendar cell content. Receives `{ details }` with `employeeId`, `date`, and `shift` data. |

---

## CalendarCell

Displays shift information in calendar views, with support for different shift types, statuses, custom colors, and loading states.

### Basic Usage

```vue
<template>
  <spr-calendar-cell
    type="standard"
    title="9:00 AM - 5:00 PM"
    description="Main Branch"
    status="approved"
    @click="handleClick"
  />
</template>

<script lang="ts" setup>
function handleClick(event) {
  console.log('Cell clicked:', event);
}
</script>
```

### Props

| Name | Type | Default | Description |
|------|------|---------|-------------|
| type | `'standard' \| 'early-morning' \| 'late-morning' \| 'afternoon' \| 'graveyard' \| 'broken' \| 'multi-break' \| 'flexible-break' \| 'flexible-weekly' \| 'flexible-daily' \| 'fixed-flexible' \| 'restday' \| 'vacation' \| 'holiday' \| 'exempt' \| 'sick' \| 'emergency'` | `'standard'` | Defines the type of calendar cell with specific color styling and label. |
| status | `'default' \| 'approved' \| 'pending' \| 'error'` | `'default'` | Controls the visual status of the cell, affecting border style. |
| title | `string` | `''` | Primary text displayed in the cell, typically time information. |
| description | `string` | `''` | Secondary text displayed in the cell, typically location information. |
| state | `'success' \| 'information' \| 'pending' \| 'caution' \| 'danger'` | `'danger'` | The state used for the status component when in error state. |
| fullwidth | `boolean` | `false` | Makes the cell take up the full width of its container. |
| viewOnly | `boolean` | `true` | When true, disables click interactions. |
| subDescription | `string` | `''` | Optional text that overrides the default shift label. |
| icon | `string` | `''` | Custom icon to override defaults for offline status types. |
| loading | `boolean` | `false` | Shows a skeletal loading animation instead of content. |
| customColor | `string` | `''` | Applies a custom border and background color (hex format recommended). |
| className | `string` | `''` | Additional CSS class names to apply. |
| customBorderSize | `string` | `''` | Applies a custom border size to the cell. |

### Events

| Name | Parameters | Description |
|------|------------|-------------|
| onClick | `(event: MouseEvent)` | Emitted when the calendar cell is clicked (only if `viewOnly` is `false`). |

### Slots

| Name | Description |
|------|-------------|
| default | Custom content, replaces the standard title, description, and shift label. |
| prefix | Custom content before the main content, replaces the status icon for offline types. |

---

## Filter

A versatile filtering solution with multi-select, advanced filter menus, infinite scrolling, search, and avatar support.

### Basic Usage

```vue
<template>
  <spr-filter v-model="selectedOptions" v-model:search="searchValue" :options="options" label="Search" hasAvatar />
</template>

<script setup>
import { ref } from 'vue';

const options = ref([
  { column: '', isSelected: false, text: 'sample 1', value: 'sample1' },
  { column: '', isSelected: false, text: 'sample 2', value: 'sample2' },
  { column: '', isSelected: false, text: 'sample 3', value: 'sample3' },
]);

const selectedOptions = ref([]);
const searchValue = ref('');
</script>
```

### Props

| Name | Type | Default | Description |
|------|------|---------|-------------|
| modelValue | `Array \| String` | `[]` | The selected filter values. Supports v-model binding. |
| options | `Array` | `[]` | The list of filter options. Each option: `{ column, isSelected, text, value, subtext?, avatar? }`. Required. |
| label | `String` | `''` | Label for the filter input field. |
| placeholder | `String` | `''` | Placeholder text for the filter input field. |
| disabled | `Boolean` | `false` | Disables the filter input. |
| filterable | `Boolean` | `false` | Enables the advanced filter menu with column-based filtering. |
| id | `String` | `'spr-filter'` | Unique identifier for the filter component. |
| filterMenu | `Array` | `[]` | List of advanced filter menu categories. Each item: `{ columnName, field, isFilterVisible?, count? }`. |
| filterData | `Array` | `[]` | Data for the advanced filter menu. |
| loading | `Boolean` | `false` | Loading state for the advanced filter menu. |
| filling | `Boolean` | `false` | Loading state for the main filter dropdown. |
| search | `String` | `''` | Search query for the main filter. Supports `v-model:search`. |
| searchFilter | `String` | `''` | Search query for the advanced filter menu. Supports `v-model:searchFilter`. |
| width | `String` | `'100%'` | Width of the filter component. |
| deselected | `String` | `''` | Value of the deselected filter option (for external deselection). |
| hasSearchApi | `Boolean` | `false` | Enables external search API integration. When true, local filtering is disabled. |
| hasAvatar | `Boolean` | `false` | Enables avatar display for filter options. |
| helperText | `String` | `''` | Helper text displayed below the filter component. |
| error | `Boolean` | `false` | Enables error state styling. |
| hasAdvancedFilterApi | `Boolean` | `false` | Enables external search API for the advanced filter menu. |

### Events

| Name | Parameters | Description |
|------|------------|-------------|
| getFilterData | `String` | Triggered when fetching filter data for a specific column. Payload is the column field name. |
| update:modelValue | `Array` | Updates the selected filter values. |
| update:search | `String` | Updates the search query for the main filter. |
| update:searchFilter | `String` | Updates the search query for the advanced filter menu. |
| selectedFilter | `Array` | Emits the selected filter options from the advanced filter menu. |
| infiniteScrollTrigger | `Boolean` | Triggered when infinite scrolling is activated for the main filter. |
| infiniteScrollFilterTrigger | `String` | Triggered when infinite scrolling is activated for the advanced filter. Payload is the current column. |

### Slots

| Name | Description |
|------|-------------|
| default | Slot for customizing the filter input field. |
| loading | Custom loading state in the advanced filter menu. |
| empty | Custom empty state in the advanced filter menu. |
| loading-state | Custom loading state in the main filter dropdown. |
| empty-state | Custom empty state in the main filter dropdown. |

---

## AttributeFilter

Provides a UI for filtering items based on attributes, using a chip trigger and list dropdown by default, with full customization support.

### Basic Usage

```vue
<template>
  <SprAttributeFilter
    id="attribute_filter1"
    :filter-label="'Status'"
    width="70px"
    popper-width="300px"
    placement="bottom-start"
    :filter-menu-list="filterList"
  />
</template>

<script lang="ts" setup>
import { ref } from 'vue';

const filterList = ref([
  { text: 'Approved', value: 'Approved' },
  { text: 'Completed', value: 'Completed' },
  { text: 'In Progress', value: 'In Progress' },
  { text: 'Pending', value: 'Pending' },
  { text: 'Rejected', value: 'Rejected' },
  { text: 'On Hold', value: 'On Hold' },
]);
</script>
```

### Props

| Name | Type | Default | Description |
|------|------|---------|-------------|
| id | `string` | `'attribute_filter'` | Unique identifier for the component. |
| filterLabel | `string` | `'Filter'` | Label displayed on the filter trigger. |
| headerLabel | `string` | `'Add Filter'` | Label displayed on the filter popper header. |
| disabled | `boolean` | `false` | When true, disables the chip component and the filter dropdown. |
| multiselect | `boolean` | `false` | Enables multiple selection of filter options. |
| filterMenuList | `{text: string, value: string}[] \| string[]` | `[]` | List of filter options to display in the dropdown. |
| searchable | `boolean` | `false` | Enable to render a search input within the filter dropdown. |
| disableLocalSearch | `boolean` | `false` | When true, disables local search functionality (for API searching). |
| showSelectedFilterCount | `boolean` | `true` | When true, displays a badge with the number of selected filters. |
| selectedFilterCount | `string` | `undefined` | Custom text for the selected filter count badge. |
| badgeVariant | `'brand' \| 'information' \| 'danger' \| 'disabled'` | `'danger'` | Variant style for the badge on the chip trigger. |
| noList | `boolean` | `false` | When true, does not display the filter list (use with body slot). |
| clearable | `boolean` | `false` | When true, renders an X icon in the chip trigger to clear selected filters. |

### Dropdown-Specific Props

| Name | Type | Default | Description |
|------|------|---------|-------------|
| placement | `'auto' \| 'auto-start' \| 'auto-end' \| 'top' \| 'top-start' \| 'top-end' \| 'right' \| 'right-start' \| 'right-end' \| 'bottom' \| 'bottom-start' \| 'bottom-end' \| 'left' \| 'left-start' \| 'left-end'` | `'bottom'` | Controls the position of the filter dropdown. |
| wrapper-position | `string` | `'relative'` | CSS position value for the filter dropdown. |
| width | `string` | `'100%'` | Width of the filter wrapper (including trigger). |
| popper-width | `string` | `'100%'` | Width of the filter dropdown. |
| popper-inner-width | `string` | `'unset'` | Width of the inner content area. |
| popper-strategy | `'absolute' \| 'fixed'` | `'absolute'` | Positioning strategy for the dropdown. |
| triggers | `('click' \| 'hover' \| 'focus' \| 'touch')[]` | `[]` | Events that trigger the dropdown to open. |
| popper-triggers | `('click' \| 'hover' \| 'focus' \| 'touch')[]` | `[]` | Events that trigger the popper element to open. |
| auto-hide | `boolean` | `true` | When true, auto-hides the dropdown when clicking outside. |

### Events

| Name | Parameters | Description |
|------|------------|-------------|
| onSaveFilter | `(savedFilters: MenuListType[])` | Emitted when clicking the save button in the default footer. |
| onOpenFilter | - | Emitted when the filter dropdown is shown. |
| onCloseFilter | - | Emitted when the filter dropdown is closed. |
| onSelectFilter | `(selectedFilters: MenuListType[])` | Emitted when selecting a filter option. |
| infiniteScrollTrigger | - | Emitted when the user scrolls to the bottom of the dropdown. |
| update:search | `(searchString: string)` | Emitted when the search input value changes. |
| onClearFilter | - | Emitted when the clear action is triggered. |

### Slots

| Name | Description |
|------|-------------|
| default | Slot for the trigger of the filter dropdown. Defaults to the chip component. |
| header | Slot for the header inside the filter dropdown. |
| actions | Slot for additional actions between the header and body. |
| body | Slot for the main content. Defaults to the list component. |
| footer | Slot for the footer. Defaults to cancel and save buttons. |
