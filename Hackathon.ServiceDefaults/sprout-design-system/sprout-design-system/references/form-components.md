# Form Components Reference

## Table of Contents

- [SprInput](#sprinput)
- [SprInputContactNumber](#sprinputcontactnumber)
- [SprInputCurrency](#sprinputcurrency)
- [SprInputDropdown](#sprinputdropdown)
- [SprInputEmail](#sprinputemail)
- [SprInputPassword](#sprinputpassword)
- [SprInputSearch](#sprinputsearch)
- [SprInputUrl](#sprinputurl)
- [SprInputUsername](#sprinputusername)
- [SprTextarea](#sprtextarea)
- [SprCheckbox](#sprcheckbox)
- [SprRadio](#sprradio)
- [SprRadioGrouped](#sprradiogrouped)
- [SprSwitch](#sprswitch)
- [SprSlider](#sprslider)
- [SprSelect](#sprselect)
- [SprSelectMultiple](#sprselectmultiple)
- [SprSelectLadderized](#sprselectladderized)
- [SprFileUpload](#sprfileupload)

---

## SprInput

UI element that allows users to enter and edit text or other data.

### Basic Usage

```vue
<template>
  <spr-input v-model="inputModel" type="text" label="Text Input" placeholder="Enter your username" />
</template>

<script lang="ts" setup>
import { ref } from 'vue';

const inputModel = ref('');
</script>
```

### Props

| Name | Type | Default | Description |
|------|------|---------|-------------|
| modelValue | string \| number | `''` | The current value of the input field. Used for v-model two-way binding. |
| type | string | `'text'` | Specifies the type of input field (text, number, email, password, search, url, tel, date, datetime-local, month, time, week, contact-number, etc.). |
| id | string | `'spr-input'` | Unique identifier for the input element. Used for label association and accessibility. |
| label | string | `''` | Text label displayed above the input field. |
| supporting-label | string | `''` | Text beside label that has a supporting style. |
| placeholder | string | `''` | Hint text displayed inside the input when empty. |
| active | boolean | `false` | When true, the input appears in its active/focused state. |
| error | boolean | `false` | When true, displays the input in an error state with error styling. |
| disabled | boolean | `false` | When true, makes the input non-interactive. |
| readonly | boolean | `false` | When true, makes the input read-only. |
| minLength | number | `undefined` | Minimum number of characters allowed. |
| maxLength | number | `undefined` | Maximum number of characters allowed. |
| showCharCount | boolean | `false` | When true, displays a character counter. Shows current/max format when used with maxLength. |
| offsetSize | `'xs'` \| `'sm'` \| `'md'` | `'sm'` | Controls spacing/offset size when using trailing content. |
| displayHelper | boolean | `false` | When true, shows the helper text area below the input. |
| helperText | string | `''` | Text content for the helper message below the input. |
| helperIcon | string | `null` | Iconify icon name to display alongside helper text. |

### Events

| Name | Parameters | Description |
|------|------------|-------------|
| update:modelValue | `(value: string \| number)` | Emitted when the input value changes. Enables v-model binding. |

### Slots

| Name | Description |
|------|-------------|
| prefix | Content at the beginning of the input field (icons, decorations). |
| trailing | Content at the end of the input field (units, text, action buttons). |
| icon | Custom icon content inside the input field. |
| helperMessage | Custom content for the helper message area. |

---

## SprInputContactNumber

International contact number input with country selector and validation. Uses `libphonenumber-js` internally for parsing and formatting on blur.

### Basic Usage

```vue
<template>
  <spr-input-contact-number id="input-contact-number-basic" v-model="inputModel" label="Input Contact Number" />
</template>

<script setup lang="ts">
import { ref } from 'vue';

const inputModel = ref('');
</script>
```

### Props

| Name | Type | Default | Description |
|------|------|---------|-------------|
| id | String | `''` | The unique id for the component. |
| v-model | String | `''` | The current (unformatted) input value. |
| placeholder | String | `'Enter Phone Number'` | Placeholder text when the input is empty. |
| pre-selected-country-code | String | `'PH'` | Initial ISO 3166-1 alpha-2 country code (e.g., PH, US). |
| disabled | Boolean | `false` | Disables the entire contact number input (including country selector). |
| disabled-country-calling-code | Boolean | `false` | Disables only the country calling code selector. |
| display-helper | Boolean | `false` | When true, displays helper text below the input. |
| helper-text | String | `''` | Helper text to display below the input. |
| helper-icon | String | `null` | Icon name to display alongside helper text. |
| error | Boolean | `false` | When true, displays the input in an error state. |

Also inherits props from the base [SprInput](#sprinput) component.

### Events

| Name | Parameters | Description |
|------|------------|-------------|
| update:model-value | `String` | Emitted whenever the raw input value changes. |
| get-selected-country-calling-code | `{ countryCode: String, countryCallingCode: String }` | Emitted after country selection changes. |
| get-parsed-international-number | `String` | Emitted with the fully parsed international number (e.g., +15551234567). |
| get-contact-number-errors | `Array<{ title: String, message: String }>` | Emitted with validation error objects if parsing detects issues. |

---

## SprInputCurrency

Currency input with selectable currency code and formatting (thousand separators, fixed decimals, symbol/code display).

### Basic Usage

```vue
<template>
  <spr-input-currency id="input-currency-basic" v-model="inputModel" label="Input Currency" />
</template>

<script setup lang="ts">
import { ref } from 'vue';

const inputModel = ref('');
</script>
```

### Props

| Name | Type | Default | Description |
|------|------|---------|-------------|
| id | String | `''` | The unique id for the component. |
| v-model | String | `''` | The raw currency string value. |
| placeholder | String | `'0.00'` | Placeholder text when the input is empty. |
| currency | String | `'PHP'` | ISO 4217 currency code (e.g., USD, EUR, PHP). Case-insensitive. Dynamically changeable. |
| pre-selected-currency | String | `'PHP'` | **Deprecated:** Use `currency` prop instead. |
| disabled | Boolean | `false` | Disables the entire currency input (including currency selector). |
| disabled-country-currency | Boolean | `false` | Disables only the currency selector. |
| auto-format | Boolean | `false` | Automatically applies thousand separators and limits decimals on blur and while typing. |
| max-decimals | Number | `2` | Maximum number of fractional digits preserved when formatting. |
| min-decimals | Number | `2` | Minimum number of fractional digits. |
| base-value | Number | `undefined` | Default numeric value to use when input is empty on blur. |
| display-as-code | Boolean | `true` | When true, shows currency code (e.g., USD). |
| display-as-symbol | Boolean | `false` | When true, shows currency symbol (e.g., $). Takes precedence over display-as-code. |
| disable-rounding | Boolean | `false` | When true, fractional digits are truncated instead of rounded. |

Also inherits props from the base [SprInput](#sprinput) component.

### Events

| Name | Parameters | Description |
|------|------------|-------------|
| update:model-value | `String` | Emitted whenever the bound input value changes. |
| get-selected-currency-meta | `{ currency: String, symbol: String, numericValue: Number \| null, rawValue: String \| null }` | Emitted after selecting a currency and on blur. |
| get-currency-value | `Number \| null` | Parsed numeric value emitted while typing, on blur, on currency change, and on mount. |

---

## SprInputDropdown

Input styled variant primarily used as trigger/display inside dropdown-based components.

### Basic Usage

```vue
<template>
  <spr-input-dropdown
    id="input-dropdown-basic"
    v-model="inputModel"
    label="Input Dropdown"
    placeholder="Select an item ..."
    readonly
  />
</template>

<script setup lang="ts">
import { ref } from 'vue';

const inputModel = ref('');
</script>
```

### Props

Shares the same props, events, slots, and validation behavior as the base [SprInput](#sprinput) component. No additional props are introduced.

---

## SprInputEmail

Email input with native browser validation patterns.

### Basic Usage

```vue
<template>
  <spr-input-email v-model="inputModel" label="Email" placeholder="Enter email" />
</template>

<script setup lang="ts">
import { ref } from 'vue';

const inputModel = ref('');
</script>
```

### Props

Shares the same props, events, slots, and validation behavior as the base [SprInput](#sprinput) component. No additional props are introduced.

---

## SprInputPassword

Password input with toggle visibility and native masking.

### Basic Usage

```vue
<template>
  <spr-input-password v-model="inputModel" label="Password" placeholder="Enter password" />
</template>

<script setup lang="ts">
import { ref } from 'vue';

const inputModel = ref('');
</script>
```

### Props

Shares the same props, events, slots, and validation behavior as the base [SprInput](#sprinput) component. No additional props are introduced.

---

## SprInputSearch

Search input optimized for filtering lists or triggering search queries.

### Basic Usage

```vue
<template>
  <spr-input-search v-model="inputModel" label="Search" placeholder="Search..." />
</template>

<script setup lang="ts">
import { ref } from 'vue';

const inputModel = ref('');
</script>
```

### Props

Shares the same props, events, slots, and validation behavior as the base [SprInput](#sprinput) component. No additional props are introduced.

---

## SprInputUrl

URL input with native browser validation for properly formatted URLs.

### Basic Usage

```vue
<template>
  <spr-input-url v-model="inputModel" label="Website" placeholder="https://example.com" />
</template>

<script setup lang="ts">
import { ref } from 'vue';

const inputModel = ref('');
</script>
```

### Props

Shares the same props, events, slots, and validation behavior as the base [SprInput](#sprinput) component. No additional props are introduced.

---

## SprInputUsername

Username-specific input with potential future validation rules.

### Basic Usage

```vue
<template>
  <spr-input-username v-model="inputModel" label="Username" placeholder="Enter username" />
</template>

<script setup lang="ts">
import { ref } from 'vue';

const inputModel = ref('');
</script>
```

### Props

Shares the same props, events, slots, and validation behavior as the base [SprInput](#sprinput) component. No additional props are introduced.

---

## SprTextarea

Multi-line text input field for entering larger amounts of text with character limits, helper messages, and error states.

### Basic Usage

```vue
<template>
  <spr-textarea v-model="textarea" label="Description" placeholder="type here...." />
</template>

<script setup lang="ts">
import { ref } from 'vue';

const textarea = ref('');
</script>
```

### Props

| Name | Type | Default | Description |
|------|------|---------|-------------|
| id | string | `''` | Unique identifier for the textarea element. |
| modelValue | string | `''` | The value of the textarea. Used with v-model for two-way binding. |
| label | string | `''` | Text label displayed above the textarea. |
| supporting-label | string | `''` | Text beside label that has a supporting style. |
| placeholder | string | `''` | Placeholder text displayed when empty. |
| active | boolean | `false` | When true, applies an active state style. |
| disabled | boolean | `false` | When true, disables the textarea. |
| readonly | boolean | `false` | When true, makes the textarea read-only. |
| error | boolean | `false` | When true, applies error state styling. |
| minLength | number | `undefined` | Minimum number of characters required. |
| maxLength | number | `undefined` | Maximum number of characters allowed. |
| rows | number | `4` | Visible height of the textarea in text lines. |
| displayHelper | boolean | `false` | When true, displays a helper message area below the textarea. |
| helperIcon | string | `null` | Iconify icon to display alongside helper message. |
| helperText | string | `''` | Text for the helper message area. |
| hasCounter | boolean | `false` | When true, displays a character counter. Only effective with maxLength. |

### Events

| Name | Parameters | Description |
|------|------------|-------------|
| update:modelValue | `(value: string)` | Emitted when the textarea value changes. |

### Slots

| Name | Description |
|------|-------------|
| helperMessage | Custom content for the helper message area. |
| counter | Custom content for the character counter area. |

---

## SprCheckbox

Allows users to select one or more items from a set of options. Supports checked, unchecked, indeterminate, and disabled states.

### Basic Usage

```vue
<template>
  <spr-checkbox v-model="checkboxBasic" label="Checkbox Label" />
</template>

<script lang="ts" setup>
import { ref } from 'vue';

const checkboxBasic = ref(false);
</script>
```

### Props

| Name | Type | Default | Description |
|------|------|---------|-------------|
| modelValue | boolean | `false` | Current state of the checkbox. Used with v-model for two-way binding. |
| label | string | `''` | Text label displayed next to the checkbox. |
| description | string | `''` | Additional explanatory text below the label. |
| disabled | boolean | `false` | When true, the checkbox becomes non-interactive. |
| checked | boolean | `false` | Controls the checked state directly (one-way binding alternative to v-model). |
| indeterminate | boolean | `false` | When true, displays a minus sign for partially checked state. |
| bordered | boolean | `false` | When true, adds a border around the entire checkbox component. |
| full-width | boolean | `false` | When true, stretches to fill the full width of its container. |

### Events

| Name | Parameters | Description |
|------|------------|-------------|
| update:modelValue | `(value: boolean)` | Emitted when the checkbox state changes. |

---

## SprRadio

A radio button that enables a user to select a single option from a set of choices.

### Basic Usage

```vue
<template>
  <div class="spr-flex spr-flex-col spr-items-start spr-gap-2">
    <spr-radio id="radio1" v-model="radioModel" name="radio_name" value="value1">Radio Label 1</spr-radio>
    <spr-radio id="radio2" v-model="radioModel" name="radio_name" value="value2">Radio Label 2</spr-radio>
  </div>
</template>

<script lang="ts" setup>
import { ref } from 'vue';

const radioModel = ref('');
</script>
```

### Props

| Name | Type | Default | Description |
|------|------|---------|-------------|
| id | string | **Required** | Unique identifier for the radio input element. |
| modelValue | string \| number \| boolean | `undefined` | Current selected value. Used with v-model for two-way binding. |
| name | string | **Required** | Name attribute. Radio buttons in the same group should share the same name. |
| value | string \| number \| boolean | **Required** | The value associated with this radio button. |
| disabled | boolean | `false` | When true, the radio button becomes non-interactive. |
| description | string | `undefined` | Additional text displayed below the radio label. |
| bordered | boolean | `false` | When true, adds a border around the entire radio component. |
| fullWidth | boolean | `false` | When true, stretches to fill the full width of its container. |
| choiceBox | boolean | `false` | When true, transforms into a choice box style with expanded clickable area. |

### Events

| Name | Parameters | Description |
|------|------------|-------------|
| update:modelValue | `(value: string \| number \| boolean)` | Emitted when the radio button is selected. |

### Slots

| Name | Description |
|------|-------------|
| default | Content displayed as the radio button's label. |

---

## SprRadioGrouped

Renders a group of radio buttons from an array of options, simplifying radio group creation.

### Basic Usage

```vue
<template>
  <spr-radio-grouped
    id="grouped-radio"
    v-model="selectedOption"
    name="grouped_options"
    :options="[
      { text: 'Option 1', value: 'value1' },
      { text: 'Option 2', value: 'value2' },
      { text: 'Option 3', value: 'value3' },
    ]"
  />
</template>

<script lang="ts" setup>
import { ref } from 'vue';

const selectedOption = ref('');
</script>
```

### Props

| Name | Type | Default | Description |
|------|------|---------|-------------|
| id | string | **Required** | Unique identifier for the radio group. Used as base for individual radio IDs. |
| model-value | string \| number \| boolean | `undefined` | Current selected value. Used with v-model. |
| name | string | **Required** | Name attribute shared by all radio buttons in the group. |
| options | RadioOption[] | `[]` | Array of radio options. Each has `text`, `value`, and optional `disabled` and `description` properties. |
| disabled | boolean | `false` | When true, all radio buttons in the group become non-interactive. |
| horizontal-align | `'left'` \| `'center'` \| `'right'` | `'left'` | Controls horizontal alignment of radio options. |
| display-helper | boolean | `false` | When true, displays helper text below the radio group. |
| helper-text | string | `''` | Helper text to display below the radio group. |
| helper-icon | string | `null` | Icon name to display alongside helper text. |
| error | boolean | `false` | When true, displays error state with red helper message text. |
| choice-box | boolean | `false` | When true, transforms each option into a full-width choice box. |

### Events

| Name | Parameters | Description |
|------|------------|-------------|
| update:modelValue | `(value: string \| number \| boolean)` | Emitted when a radio option is selected. |

### RadioOption Interface

```typescript
interface RadioOption {
  text: string;
  value: string | number | boolean;
  disabled?: boolean;
  description?: string;
}
```

---

## SprSwitch

Switch component to show a boolean state (similar to a checkbox).

### Basic Usage

```vue
<template>
  <spr-switch v-model="switchValue">Switch</spr-switch>
</template>

<script lang="ts" setup>
import { ref } from 'vue';

const switchValue = ref(false);
</script>
```

### Props

| Name | Type | Default | Description |
|------|------|---------|-------------|
| modelValue | boolean | `false` | The current value of the switch (on/off state). |
| disabled | boolean | `false` | When true, the switch cannot be interacted with. |
| state | `'default'` \| `'hover'` \| `'pressed'` \| `'disabled'` | `'default'` | Controls the visual state of the switch. |
| id | string | `''` | HTML id attribute for the switch input element. |

### Events

| Name | Parameters | Description |
|------|------------|-------------|
| update:modelValue | `(value: boolean)` | Emitted when the switch state changes. Uses `@vueuse/core` useVModel. |

### Slots

| Name | Description |
|------|-------------|
| default | Text to the left of the switch. If both default and leftText are provided, leftText takes precedence. |
| leftText | Explicitly places text to the left of the switch. |
| rightText | Displays text to the right of the switch. |

---

## SprSlider

Allows users to select a value from a range by dragging a handle. Supports min/max values, step increments, and size variations.

### Basic Usage

```vue
<template>
  <spr-slider :min="0" :max="100" :step="1" v-model="sliderValue" />
</template>

<script setup>
import { ref } from 'vue';

const sliderValue = ref(0);
</script>
```

### Props

| Name | Type | Default | Description |
|------|------|---------|-------------|
| modelValue | number | `0` | The current value of the slider. Used with v-model. |
| size | `'lg'` \| `'sm'` | `'lg'` | Controls the size of the slider component. |
| min | number | `0` | Minimum value (leftmost position). |
| max | number | `100` | Maximum value (rightmost position). |
| step | number | `1` | Increment between values on the slider. |
| disabled | boolean | `false` | When true, the slider becomes non-interactive. |

### Events

| Name | Parameters | Description |
|------|------------|-------------|
| update:modelValue | `(value: number)` | Emitted continuously as the user drags the thumb. |
| slideend | `(value: number)` | Emitted when the user completes a sliding interaction (releases pointer). |

---

## SprSelect

Single-select dropdown that allows users to choose one option from a list.

### Basic Usage

```vue
<template>
  <spr-select
    id="select-basic"
    v-model="selectModel"
    label="Select Label"
    placeholder="Select an option"
    :options="options"
  />
</template>

<script lang="ts" setup>
import { ref } from 'vue';

const selectModel = ref('');

const options = ref([
  { text: 'Apple', value: 'apple' },
  { text: 'Banana', value: 'banana' },
  { text: 'Cherry', value: 'cherry' },
]);
</script>
```

### Props

| Name | Type | Default | Description |
|------|------|---------|-------------|
| id | String | - | Required. Binds popper within the select. |
| v-model | String \| Number \| Object \| Array | `[]` | Value binding. Accepts single primitives, objects, or arrays. |
| options | optionsType[] \| string[] \| object[] | `[]` | List of options with `text` and `value` properties. |
| group-items-by | `'A-Z'` \| `'Z-A'` | - | Group items by alphabetical order. |
| text-field | String | `'text'` | Field name for display text when using object arrays. |
| value-field | String | `'value'` | Field name for value when using object arrays. |
| display-text | String | - | Display text for initial load (useful for API-driven selects). |
| placeholder | String | - | Placeholder text for the input. |
| label | String | `''` | Label for the select input. |
| supporting-label | string | `''` | Text beside label with supporting style. |
| input-loader | Boolean | `false` | Displays a loading spinner inside the select input. |
| placement | String | `'bottom'` | Placement of the select popper. |
| searchable | Boolean | `false` | Allow typing in the input to search/filter options. |
| disabled-local-search | Boolean | `false` | Disables local search (for API-only search). |
| triggers | Array | `['click']` | Events that trigger the dropdown to open. |
| popper-triggers | Array | `[]` | Events that trigger the popper element to open. |
| popper-strategy | String | `'absolute'` | Popper positioning strategy: 'absolute' or 'fixed'. |
| popper-width | String | `'100%'` | Width of the select's popper. |
| popper-container | String \| HTMLElement | `''` | Custom container for the popper element. |
| auto-hide | Boolean | `true` | When true, hides dropdown when clicking outside. |
| width | String | `'100%'` | Width of the select component wrapper. |
| wrapper-position | String | `'relative'` | CSS position of the select wrapper. |
| disabled | Boolean | `false` | Disables the select. |
| clearable | Boolean | `false` | Allows clearing the selected value. |
| options-loader | Boolean | `false` | Displays skeletal loading inside the popper. |
| infinite-scroll-loader | Boolean | `false` | Displays loading spinner at bottom of list for infinite scroll. |
| lozenge | Boolean | `false` | Enables lozenge mode for list items. |
| item-icon | String | `''` | Iconify icon for list items (e.g., ph:trash). |
| avatar-variant | String | - | Avatar type for options: 'user', 'user-light', 'building', 'building-light'. |

### Events

| Name | Parameters | Description |
|------|------------|-------------|
| update:model-value | Any | Emitted when the model value changes. |
| infinite-scroll-trigger | None | Emitted when scrolled to the bottom (for dynamic data loading). |
| search-string | String | Emitted when typing in the search input. |
| get-selected-option | Object | Emitted to get the selected option. |
| popper-state | Boolean | Emitted when the popper opens or closes. |

### Exposed Methods

| Method | Description |
|--------|-------------|
| handleClear() | Clears the current selection. |

---

## SprSelectMultiple

Multi-select dropdown that allows users to select multiple options from a list.

### Basic Usage

```vue
<template>
  <spr-select-multiple
    id="select-multiple-basic"
    v-model="multiSelectBasic"
    label="Multi-Select Label"
    placeholder="Select an option"
    :options="options"
  />
</template>

<script lang="ts" setup>
import { ref } from 'vue';

const multiSelectBasic = ref([]);

const options = ref([
  { text: 'Apple', value: 'apple' },
  { text: 'Banana', value: 'banana' },
  { text: 'Cherry', value: 'cherry' },
]);
</script>
```

### Props

| Name | Type | Default | Description |
|------|------|---------|-------------|
| id | String | - | Required. Binds popper within the select. |
| v-model | Array | `[]` | Value binding. Always use an array for multi-select. |
| search-value | String | `''` | Value binding for the search input. |
| options | Array | `[]` | List of options with `text` and `value` fields. |
| group-items-by | String | - | Group items: 'A-Z' or 'Z-A'. |
| text-field | String | `'text'` | Field name for display text with object arrays. |
| value-field | String | `'value'` | Field name for value with object arrays. |
| placeholder | String | - | Placeholder text for the input. |
| label | String | `''` | Label for the select input. |
| supporting-label | string | `''` | Text beside label with supporting style. |
| input-loader | Boolean | `false` | Displays a loading spinner inside the select input. |
| placement | String | `'bottom'` | Popper placement. |
| triggers | Array | `['click']` | Events that trigger the dropdown. |
| popper-triggers | Array | `[]` | Events that trigger the popper element. |
| auto-hide | Boolean | `true` | Hides dropdown when clicking outside. |
| popper-strategy | String | `'absolute'` | Popper positioning strategy. |
| popper-width | String | `'100%'` | Width of the popper element. |
| popper-container | String \| HTMLElement | `''` | Custom container for the popper element. |
| width | String | `'100%'` | Width of the select wrapper. |
| wrapper-position | String | `'relative'` | CSS position of the wrapper. |
| display-text | String | - | Display text for the input (useful for async/infinite scroll). |
| supporting-display-text | String | - | Custom text inside the list. |
| persistent-display-text | Boolean | `false` | If true, displayText always shows even when selections change. |
| display-selected-count-only | Boolean | `false` | Displays selected item counter instead of text. |
| display-list-item-selected | Boolean | `false` | Displays selected item indicator inside the list. |
| allow-select-all | Boolean | `false` | Enables "Select All" / "Unselect All" button. |
| display-helper | Boolean | `false` | Show helper text below the input. |
| helper-icon | String | `null` | Icon for helper text. |
| helper-text | String | `''` | Helper text below the input. |
| disabled | Boolean | `false` | Disable the select. |
| clearable | Boolean | `false` | Show clear button to remove all selections. |
| chipped | Boolean | `false` | Display selected items as chips. |
| searchable | Boolean | `false` | Enable searching within options. |
| disabled-local-search | Boolean | `false` | Disable local search functionality. |
| options-loader | Boolean | `false` | Displays skeletal loading inside the popper. |
| infinite-scroll-loader | Boolean | `false` | Displays loading spinner at bottom for infinite scroll. |
| lozenge | Boolean | `false` | Enables lozenge mode for list items. |
| item-icon | String | `''` | Iconify icon for list items. |

### Events

| Name | Parameters | Description |
|------|------------|-------------|
| update:modelValue | Array | Emitted when the selection changes. |
| popper-state | Boolean | Emitted when the popper opens or closes. |
| search-string | String | Emitted when typing in the search input. |
| get-selected-options | Object | Emitted when items are selected. |
| get-single-selected-item | Object | Emitted when a single option is selected. |

### Exposed Methods

| Method | Description |
|--------|-------------|
| handleClear() | Clears the current selection. |

---

## SprSelectLadderized

Ladderized select for options organized in hierarchical groups. Supports deeply nested sublevels and subtext for each item.

### Basic Usage

```vue
<template>
  <spr-select-ladderized
    id="select-ladderized-basic"
    v-model="ladderizedModel"
    :options="options"
    label="Ladderized Select"
    placeholder="Select an item"
  />
</template>

<script lang="ts" setup>
import { ref } from 'vue';

const ladderizedModel = ref([]);

const options = ref([
  { text: 'Tiger', value: 'tiger', subtext: 'Rawr of the jungle' },
  {
    text: 'Lion', value: 'lion', subtext: 'King of the jungle',
    sublevel: [
      { text: 'Cub', value: 'cub', subtext: 'Young lion' },
      { text: 'Pride Member', value: 'pride-member', subtext: 'Member of a lion pride' },
    ],
  },
]);
</script>
```

### Props

| Name | Type | Default | Description |
|------|------|---------|-------------|
| id | String | - | Required. Unique id for the select. |
| v-model | Array | `[]` | Value binding. Array of strings representing the path to the selected item. |
| options | Array | `[]` | List of options with optional `sublevel` for hierarchy. Each item has `text`, `value`, optional `subtext` and `sublevel`. |
| label | String | `''` | Label for the select input. |
| supporting-label | string | `''` | Text beside label with supporting style. |
| input-loader | Boolean | `false` | Shows loading spinner while options are being fetched. |
| placeholder | String | `''` | Input placeholder. |
| text-seperator | String | `' > '` | Separator between values in the ladderized text input. |
| prepend-text | Boolean | `false` | Prepend the text in the ladderized text input. |
| helper-text | String | `''` | Helper text below input. |
| helper-icon | String | `null` | Icon for helper text. |
| display-helper | Boolean | `false` | Show helper text. |
| clearable | Boolean | `false` | Show clear (x) icon to remove value. |
| placement | String | `'bottom'` | Popper placement. |
| triggers | Array | `['click']` | Events that trigger the dropdown. |
| popper-triggers | Array | `[]` | Events that trigger the popper element. |
| auto-hide | Boolean | `true` | Hides dropdown when clicking outside. |
| wrapper-position | String | `'relative'` | CSS position of wrapper. |
| width | String | `'100%'` | Width of the select wrapper. |
| popper-width | String | `'100%'` | Width of the select popper. |
| popper-strategy | String | `'absolute'` | Popper positioning strategy. |
| popper-container | String \| HTMLElement | `''` | Custom container for the popper element. |
| disabled | Boolean | `false` | Disable the select. |
| remove-current-level-in-back-label | Boolean | `false` | Hide current level in back label. |
| searchable-options | Boolean | `false` | Enable search input for filtering options. |
| searchable-options-placeholder | String | `'Search...'` | Placeholder for the search input. |
| writableInputText | Boolean | `false` | Enable writable input text. Does not affect search. |
| options-loader | Boolean | `false` | Displays skeletal loading inside the popper. |
| infinite-scroll-loader | Boolean | `false` | Displays loading spinner at bottom for infinite scroll. |

### Events

| Name | Parameters | Description |
|------|------------|-------------|
| update:modelValue | Array \| String \| Number \| Object | Emitted when the selection changes. |
| popper-state | Boolean | Emitted when the popper opens or closes. |

---

## SprFileUpload

File upload component with drag-and-drop and file selection support. Validates file types and sizes with visual error feedback.

### Basic Usage

```vue
<template>
  <spr-file-upload
    v-model="files"
    :file-types="['image/jpeg', 'image/png']"
    :max-file-size="10"
    title="Attachments"
    multiple
  />
</template>

<script lang="ts" setup>
import { ref } from 'vue';

const files = ref([]);
</script>
```

### Props

| Name | Type | Default | Description |
|------|------|---------|-------------|
| type | `'default'` \| `'center'` | `'default'` | Layout style: 'default' (horizontal) or 'center' (vertical with prominent drop area). |
| title | string | `undefined` | Title/label displayed above the file upload component. |
| modelValue | File[] | `[]` | Array of uploaded files, bound with v-model. |
| multiple | boolean | `false` | When true, allows uploading multiple files at once. |
| disabled | boolean | `false` | When true, disables file upload functionality. |
| maxFileSize | number | `10` | Maximum allowed file size in megabytes (MB). |
| fileTypes | string[] | All common document and image types | Array of MIME types allowed for upload. |
| showError | boolean | `false` | When true, displays error messages for invalid files. |
| errorMessages | string[] | `[]` | Array of error messages for validation failures. |
| hideFilePreviewIcon | boolean | `false` | When true, hides the check icon next to uploaded files. |
| hideDropzoneLabel | boolean | `false` | When true, hides the "drop your files to upload" label. |
| supportedFileTypeLabel | string | Auto-generated | Custom text for supported file types display. |
| showProgress | boolean | `false` | When true, displays a progress bar for upload status. |
| progressValue | number | `0` | Progress percentage value (0-100). |

### Events

| Name | Parameters | Description |
|------|------------|-------------|
| update:modelValue | `File[]` | Emitted when files are added, replaced, or removed. |
| validation-error | `string[]` | Emitted when files fail validation. Emits empty array when all files are valid. |
