# Design Tokens & Utilities Reference

> **Important:** If the consuming app has its own Tailwind CSS setup, always prefer standard (or app-prefixed) Tailwind utility classes (e.g., `flex`, `gap-4`, `p-2`, `w-full`, `grid`, `rounded-lg`) for general layout, spacing, sizing, display, flexbox, grid, and other standard utilities. Only use `spr-` prefixed classes for the following specific design system tokens: **colors, typography, border-color, border-radius, max-width, and skeletal loaders**.

## Table of Contents

- [Colors](#colors)
- [Text Colors](#text-colors)
- [Background Colors](#background-colors)
- [Typography](#typography)
- [Spacing](#spacing)
- [Border Radius](#border-radius)
- [Border Colors](#border-colors)
- [Box Shadows](#box-shadows)
- [Max Width](#max-width)
- [Skeletal Loader](#skeletal-loader)
- [Miscellaneous](#miscellaneous)

---

## Colors

Overview of the color scheme used in the design system (Toge Design System v3.0), including primary colors and their variants. All classes use the `spr-` prefix.

### Available Color Palettes

Each palette has shades from 50 to 950. The naming convention for color utility classes is `spr-{property}-{palette}-{shade}` (e.g., `spr-text-kangkong-700`, `spr-bg-tomato-500`).

| Palette | Description | Shade Range |
|---|---|---|
| `white` | Neutral whites and grays | 50 (#FFFFFF) to 950 (#292929) |
| `mushroom` | Neutral earthy tones | 50 (#EFF1F1) to 950 (#262B2B) |
| `tomato` | Red/danger tones | 50 (#FEF2F3) to 950 (#440B0E) |
| `carrot` | Orange/caution tones | 50 (#FFFAEC) to 950 (#461C04) |
| `mango` | Yellow/pending tones | 50 (#FFFFEA) to 950 (#482200) |
| `kangkong` | Green/success/brand tones | 50 (#F0FDF4) to 950 (#052E15) |
| `wintermelon` | Cyan/accent tones | 50 (#ECFEFF) to 950 (#073345) |
| `blueberry` | Blue/information tones | 50 (#EEF7FF) to 950 (#122E59) |
| `ubas` | Purple/violet tones | 50 (#F5F3FF) to 950 (#311065) |

### Color Shade Values

All palettes follow the same shade scale: **50, 100, 200, 300, 400, 500, 600, 700, 800, 900, 950**.

#### white

| Shade | Hex |
|---|---|
| 50 | #FFFFFF |
| 100 | #F1F2F3 |
| 200 | #DBDBDD |
| 300 | #BABCC0 |
| 400 | #989898 |
| 500 | #7C7C7C |
| 600 | #656565 |
| 700 | #525252 |
| 800 | #464646 |
| 900 | #3D3D3D |
| 950 | #292929 |

#### mushroom

| Shade | Hex |
|---|---|
| 50 | #EFF1F1 |
| 100 | #E6EAEA |
| 200 | #D9DEDE |
| 300 | #B8C1C0 |
| 400 | #919F9D |
| 500 | #738482 |
| 600 | #5D6C6B |
| 700 | #4C5857 |
| 800 | #414B4B |
| 900 | #394141 |
| 950 | #262B2B |

#### tomato

| Shade | Hex |
|---|---|
| 50 | #FEF2F3 |
| 100 | #FEE2E3 |
| 200 | #FDCBCE |
| 300 | #FBA6AA |
| 400 | #F6737A |
| 500 | #EC4750 |
| 600 | #DA2F38 |
| 700 | #B61F27 |
| 800 | #971D23 |
| 900 | #7D1F24 |
| 950 | #440B0E |

#### carrot

| Shade | Hex |
|---|---|
| 50 | #FFFAEC |
| 100 | #FFF4D3 |
| 200 | #FFE5A5 |
| 300 | #FFD16D |
| 400 | #FFB132 |
| 500 | #FF970A |
| 600 | #FF7F00 |
| 700 | #CC5C02 |
| 800 | #A1470B |
| 900 | #823C0C |
| 950 | #461C04 |

#### mango

| Shade | Hex |
|---|---|
| 50 | #FFFFEA |
| 100 | #FFFBC5 |
| 200 | #FFF885 |
| 300 | #FFED46 |
| 400 | #FFDF1B |
| 500 | #FFBF00 |
| 600 | #E29300 |
| 700 | #BB6802 |
| 800 | #985008 |
| 900 | #7C420B |
| 950 | #482200 |

#### kangkong

| Shade | Hex |
|---|---|
| 50 | #F0FDF4 |
| 100 | #DCFCE6 |
| 200 | #BBF7CE |
| 300 | #86EFA8 |
| 400 | #4ADE7B |
| 500 | #22C558 |
| 600 | #17AD49 |
| 700 | #158039 |
| 800 | #166531 |
| 900 | #14532B |
| 950 | #052E15 |

#### wintermelon

| Shade | Hex |
|---|---|
| 50 | #ECFEFF |
| 100 | #CEFBFF |
| 200 | #A3F3FE |
| 300 | #64E9FC |
| 400 | #1ED5F2 |
| 500 | #02AFCE |
| 600 | #0592B5 |
| 700 | #0C7492 |
| 800 | #135E77 |
| 900 | #154E64 |
| 950 | #073345 |

#### blueberry

| Shade | Hex |
|---|---|
| 50 | #EEF7FF |
| 100 | #D8EBFF |
| 200 | #BADCFF |
| 300 | #8BBDFF |
| 400 | #549EFF |
| 500 | #2D88FF |
| 600 | #1679FA |
| 700 | #0F6EEB |
| 800 | #1356BA |
| 900 | #164B92 |
| 950 | #122E59 |

#### ubas

| Shade | Hex |
|---|---|
| 50 | #F5F3FF |
| 100 | #EEE9FE |
| 200 | #DED6FE |
| 300 | #C6B5FD |
| 400 | #AA8BFA |
| 500 | #8952F6 |
| 600 | #8139EE |
| 700 | #7227DA |
| 800 | #5F21B6 |
| 900 | #501D95 |
| 950 | #311065 |

---

## Text Colors

Semantic text color utility classes for consistent text color application across the UI.

### Base Text Colors

| Class | Description | Color Token |
|---|---|---|
| `.spr-text-color-strong` | Strong emphasis text | `spr-text-mushroom-950` |
| `.spr-text-color-base` | Base text for normal content | `spr-text-mushroom-600` |
| `.spr-text-color-supporting` | Supporting text | `spr-text-mushroom-500` |
| `.spr-text-color-weak` | Weak emphasis text | `spr-text-mushroom-400` |
| `.spr-text-color-disabled` | Disabled elements | `spr-text-white-400` |
| `.spr-text-color-on-fill-disabled` | On filled disabled elements | `spr-text-white-500` |

### Inverted Text Colors (for dark backgrounds)

| Class | Description | Color Token |
|---|---|---|
| `.spr-text-color-inverted-strong` | Strong text on dark | `spr-text-white-50` |
| `.spr-text-color-inverted-base` | Base text on dark | `spr-text-mushroom-200` |
| `.spr-text-color-inverted-weak` | Weak text on dark | `spr-text-mushroom-400` |
| `.spr-text-color-inverted-disabled` | Disabled text on dark | `spr-text-white-600` |

### Semantic Text Colors

Each semantic category (brand, success, information, danger, pending, caution, accent) has three states: `-base`, `-hover`, `-pressed`.

| Category | Base Class | Color Token |
|---|---|---|
| Brand | `.spr-text-color-brand-base` | `spr-text-kangkong-700` |
| Success | `.spr-text-color-success-base` | `spr-text-kangkong-700` |
| Information | `.spr-text-color-information-base` | `spr-text-blueberry-800` |
| Danger | `.spr-text-color-danger-base` | `spr-text-tomato-700` |
| Pending | `.spr-text-color-pending-base` | `spr-text-mango-800` |
| Caution | `.spr-text-color-caution-base` | `spr-text-carrot-800` |
| Accent | `.spr-text-color-accent-base` | `spr-text-wintermelon-700` |

---

## Background Colors

Semantic background color utility classes for consistent background color application.

### Base Background Colors

| Class | Description | Color Token |
|---|---|---|
| `.spr-background-color` | Primary background | `spr-bg-white-50` |
| `.spr-background-color-default` | Default background | `spr-bg-mushroom-200` |
| `.spr-background-color-surface` | Surface background | `spr-bg-mushroom-50` |
| `.spr-background-color-surface-adaptive` | Adaptive surface with opacity | `spr-bg-mushroom-950/[0.05]` |
| `.spr-background-color-base` | Base background | `spr-bg-mushroom-100` |

### Interactive States

| Class | Description | Color Token |
|---|---|---|
| `.spr-background-color-hover` | Hover state | `spr-bg-mushroom-950/[0.08]` |
| `.spr-background-color-pressed` | Pressed state | `spr-bg-mushroom-950/[0.12]` |
| `.spr-background-color-disabled` | Disabled elements | `spr-bg-white-100` |

### Inverted Background Colors

| Class | Description | Color Token |
|---|---|---|
| `.spr-background-color-inverted` | Inverted (dark) background | `spr-bg-mushroom-950` |
| `.spr-background-color-inverted-hover` | Inverted hover | `spr-bg-mushroom-900` |
| `.spr-background-color-inverted-pressed` | Inverted pressed | `spr-bg-mushroom-800` |

### Active States

| Class | Description | Color Token |
|---|---|---|
| `.spr-background-color-single-active` | Single active item | `spr-bg-kangkong-100` |
| `.spr-background-color-multiple-active` | Multiple active items | `spr-bg-kangkong-50` |

### Semantic Background Colors

Each semantic category has six variants: `-base`, `-hover`, `-pressed`, `-weak`, `-weak-hover`, `-weak-pressed`.

| Category | Base Class | Weak Class |
|---|---|---|
| Brand | `.spr-background-color-brand-base` | `.spr-background-color-brand-weak` |
| Success | `.spr-background-color-success-base` | `.spr-background-color-success-weak` |
| Information | `.spr-background-color-information-base` | `.spr-background-color-information-weak` |
| Danger | `.spr-background-color-danger-base` | `.spr-background-color-danger-weak` |
| Pending | `.spr-background-color-pending-base` | `.spr-background-color-pending-weak` |
| Caution | `.spr-background-color-caution-base` | `.spr-background-color-caution-weak` |
| Accent | `.spr-background-color-accent-base` | `.spr-background-color-accent-weak` |

---

## Typography

Font conventions to ensure consistent and optimal presentation across various platforms. These classes reflect the visual identity and design principles of the Toge Design System.

### Font Family

| Font Family | Class | Usage |
|---|---|---|
| Rubik | `spr-font-main` | Main typeface for general UI text. Modern, rounded design for clarity and smooth readability. |
| Roboto | `spr-font-inbound` | Used specifically for inbound products from SAIL. |
| Roboto Mono | `spr-font-code` | Used for code blocks and technical content to distinguish from body text. |

### Font Sizes

| Size | Class |
|---|---|
| 12px | `spr-font-size-100` |
| 14px | `spr-font-size-200` |
| 16px | `spr-font-size-300` |
| 20px | `spr-font-size-400` |
| 24px | `spr-font-size-500` |
| 28px | `spr-font-size-600` |
| 32px | `spr-font-size-700` |
| 40px | `spr-font-size-800` |
| 48px | `spr-font-size-900` |
| 56px | `spr-font-size-1000` |

### Font Weight

| Weight | Class |
|---|---|
| 100 | `spr-font-thin` |
| 200 | `spr-font-extralight` |
| 300 | `spr-font-light` |
| 400 | `spr-font-normal` |
| 500 | `spr-font-medium` |
| 600 | `spr-font-semibold` |
| 700 | `spr-font-bold` |
| 800 | `spr-font-extrabold` |
| 900 | `spr-font-black` |

### Line Height

| Line Height | Class |
|---|---|
| 12px | `spr-line-height-100` |
| 14px | `spr-line-height-200` |
| 16px | `spr-line-height-300` |
| 20px | `spr-line-height-400` |
| 24px | `spr-line-height-500` |
| 32px | `spr-line-height-600` |
| 36px | `spr-line-height-700` |
| 40px | `spr-line-height-800` |
| 48px | `spr-line-height-900` |
| 60px | `spr-line-height-1000` |

### Letter Spacing

| Spacing | Class |
|---|---|
| -1.3px | `spr-letter-spacing-densest` |
| -1px | `spr-letter-spacing-denser` |
| -0.7px | `spr-letter-spacing-dense` |
| 0px | `spr-letter-spacing-normal` |
| 0.7px | `spr-letter-spacing-wide` |
| 1px | `spr-letter-spacing-wider` |
| 1.3px | `spr-letter-spacing-widest` |

### Headings

Composite typography classes that combine font size, line height, letter spacing, font family, and font weight.

| Class | Font Size | Line Height | Letter Spacing | Font | Weight |
|---|---|---|---|---|---|
| `spr-heading-xl` | spr-font-size-900 | spr-line-height-1000 | spr-letter-spacing-densest | spr-font-main | spr-font-medium |
| `spr-heading-lg` | spr-font-size-800 | spr-line-height-900 | spr-letter-spacing-denser | spr-font-main | spr-font-medium |
| `spr-heading-md` | spr-font-size-700 | spr-line-height-800 | spr-letter-spacing-denser | spr-font-main | spr-font-medium |
| `spr-heading-sm` | spr-font-size-600 | spr-line-height-700 | spr-letter-spacing-dense | spr-font-main | spr-font-medium |
| `spr-heading-xs` | spr-font-size-500 | spr-line-height-600 | spr-letter-spacing-dense | spr-font-main | spr-font-medium |

### Subheadings

| Class | Font Size | Line Height | Letter Spacing | Font | Weight |
|---|---|---|---|---|---|
| `spr-subheading-sm` | spr-font-size-400 | spr-line-height-500 | spr-letter-spacing-dense | spr-font-main | spr-font-medium |
| `spr-subheading-xs` | spr-font-size-300 | spr-line-height-400 | spr-letter-spacing-normal | spr-font-main | spr-font-medium |

### Body Text

Body text classes use `spr-font-main` and `spr-letter-spacing-normal`. Each size has variants for regular, regular-underline, medium, and medium-underline.

| Base Class | Font Size | Line Height | Variants |
|---|---|---|---|
| `spr-body-lg` | spr-font-size-400 | spr-line-height-600 | `spr-body-lg-regular`, `spr-body-lg-regular-underline`, `spr-body-lg-regular-medium`, `spr-body-lg-medium-underline` |
| `spr-body-md` | spr-font-size-300 | spr-line-height-500 | `spr-body-md-regular`, `spr-body-md-regular-underline`, `spr-body-md-regular-medium`, `spr-body-md-medium-underline` |
| `spr-body-sm` | spr-font-size-200 | spr-line-height-400 | `spr-body-sm-regular`, `spr-body-sm-regular-underline`, `spr-body-sm-regular-medium`, `spr-body-sm-medium-underline` |
| `spr-body-xs` | spr-font-size-100 | spr-line-height-300 | `spr-body-xs-regular`, `spr-body-xs-regular-underline`, `spr-body-xs-regular-medium`, `spr-body-xs-medium-underline` |

Variant naming convention:
- `*-regular` = base + `spr-font-normal`
- `*-regular-underline` = base + `spr-font-normal` + `spr-decoration-solid`
- `*-regular-medium` = base + `spr-font-medium`
- `*-medium-underline` = base + `spr-font-medium` + `spr-decoration-solid`

### Labels

Labels use `spr-font-main` and `spr-letter-spacing-wide`. Used for form inputs, navigation, and UI elements.

| Base Class | Font Size | Line Height | Variants |
|---|---|---|---|
| `spr-label-sm` | spr-font-size-200 | spr-line-height-200 | `spr-label-sm-regular`, `spr-label-sm-medium` |
| `spr-label-xs` | spr-font-size-100 | spr-line-height-100 | `spr-label-xs-regular`, `spr-label-xs-medium` |

Variant naming convention:
- `*-regular` = base + `spr-font-normal`
- `*-medium` = base + `spr-font-medium`

---

## Spacing

Spacing ensures consistent margins, padding, gaps, etc. across various components. The spacing classes are derived from root CSS variables and extend Tailwind's default spacing scale.

### Available Values

| Value | Root Variable | Class Token |
|---|---|---|
| 2px | `--size-50` | `size-spacing-6xs` |
| 4px | `--size-100` | `size-spacing-5xs` |
| 6px | `--size-150` | `size-spacing-4xs` |
| 8px | `--size-200` | `size-spacing-3xs` |
| 12px | `--size-250` | `size-spacing-2xs` |
| 16px | `--size-300` | `size-spacing-xs` |
| 24px | `--size-400` | `size-spacing-sm` |
| 32px | `--size-500` | `size-spacing-md` |
| 40px | `--size-600` | `size-spacing-lg` |
| 48px | `--size-700` | `size-spacing-xl` |
| 64px | `--size-800` | `size-spacing-2xl` |
| 72px | `--size-900` | `size-spacing-3xl` |
| 80px | `--size-1000` | `size-spacing-4xl` |
| 96px | `--size-1100` | `size-spacing-5xl` |
| 128px | `--size-1200` | `size-spacing-6xl` |

### Usage

These values extend Tailwind's spacing scale and are inherited by: **padding**, **margin**, **width**, **minWidth**, **maxWidth**, **height**, **minHeight**, **maxHeight**, **gap**, **inset**, **space**, **translate**, **scrollMargin**, and **scrollPadding**.

> **Note:** For general spacing, prefer standard Tailwind utilities (`p-4`, `m-2`, `gap-4`, etc.) from the app's own Tailwind config. Only use `spr-` prefixed spacing tokens when you need the exact design system spacing values listed above.

Append the `size-spacing-*` token to any Tailwind spacing utility with the `spr-` prefix when using design system spacing tokens:

| Class Example | Result |
|---|---|
| `spr-m-size-spacing-xs` | margin: 16px |
| `spr-mb-size-spacing-md` | margin-bottom: 32px |
| `spr-px-size-spacing-4xs` | padding-left: 6px; padding-right: 6px |

---

## Border Radius

Utility classes for setting border radius on elements for consistent rounded corners.

### Available Classes

| Value | Root Variable | Class |
|---|---|---|
| 2px | `--size-50` | `spr-rounded-border-radius-2xs` |
| 4px | `--size-100` | `spr-rounded-border-radius-xs` |
| 6px | `--size-150` | `spr-rounded-border-radius-sm` |
| 8px | `--size-200` | `spr-rounded-border-radius-md` |
| 12px | `--size-250` | `spr-rounded-border-radius-lg` |
| 16px | `--size-300` | `spr-rounded-border-radius-xl` |
| 999px | - | `spr-rounded-border-radius-full` |

---

## Border Colors

Semantic border color utility classes for various states and contexts. Ensures consistent border color application across the UI.

### Border Color Classes

| Class | Description | Color Token |
|---|---|---|
| `.spr-border-color-strong` | Strong border color for high emphasis | spr-border-mushroom-500 |
| `.spr-border-color-supporting` | Supporting border color | spr-border-mushroom-400 |
| `.spr-border-color-base` | Base border color for default state | spr-border-mushroom-300 |
| `.spr-border-color-hover` | Border color on hover state | spr-border-mushroom-400 |
| `.spr-border-color-pressed` | Border color on pressed state | spr-border-mushroom-500 |
| `.spr-border-color-weak` | Weak border color for subtle separation | spr-border-mushroom-200 |
| `.spr-border-color-disabled` | Border color for disabled elements | spr-border-white-100 |
| `.spr-border-color-on-fill-disabled` | Border color on disabled filled backgrounds | spr-border-white-200 |
| `.spr-border-color-brand-base` | Brand border color (base state) | spr-border-kangkong-700 |
| `.spr-border-color-brand-hover` | Brand border color (hover state) | spr-border-kangkong-800 |
| `.spr-border-color-brand-pressed` | Brand border color (pressed state) | spr-border-kangkong-900 |
| `.spr-border-color-success-base` | Success border color (base state) | spr-border-kangkong-700 |
| `.spr-border-color-success-hover` | Success border color (hover state) | spr-border-kangkong-800 |
| `.spr-border-color-success-pressed` | Success border color (pressed state) | spr-border-kangkong-900 |
| `.spr-border-color-information-base` | Information border color (base state) | spr-border-blueberry-700 |
| `.spr-border-color-information-hover` | Information border color (hover state) | spr-border-blueberry-800 |
| `.spr-border-color-information-pressed` | Information border color (pressed state) | spr-border-blueberry-900 |
| `.spr-border-color-danger-base` | Danger border color (base state) | spr-border-tomato-600 |
| `.spr-border-color-danger-hover` | Danger border color (hover state) | spr-border-tomato-700 |
| `.spr-border-color-danger-pressed` | Danger border color (pressed state) | spr-border-tomato-800 |
| `.spr-border-color-pending-base` | Pending border color (base state) | spr-border-mango-700 |
| `.spr-border-color-pending-hover` | Pending border color (hover state) | spr-border-mango-800 |
| `.spr-border-color-pending-pressed` | Pending border color (pressed state) | spr-border-mango-900 |
| `.spr-border-color-caution-base` | Caution border color (base state) | spr-border-carrot-700 |
| `.spr-border-color-caution-hover` | Caution border color (hover state) | spr-border-carrot-800 |
| `.spr-border-color-caution-pressed` | Caution border color (pressed state) | spr-border-carrot-900 |
| `.spr-border-color-accent-base` | Accent border color (base state) | spr-border-wintermelon-700 |
| `.spr-border-color-accent-hover` | Accent border color (hover state) | spr-border-wintermelon-800 |
| `.spr-border-color-accent-pressed` | Accent border color (pressed state) | spr-border-wintermelon-900 |

### Divider Color Classes (Between Children)

Utility classes for setting divider (border) colors between child elements.

| Class | Description | Color Token |
|---|---|---|
| `.spr-divide-color-strong` | Strong divider color for high emphasis | spr-divide-mushroom-500 |
| `.spr-divide-color-supporting` | Supporting divider color | spr-divide-mushroom-400 |
| `.spr-divide-color-base` | Base divider color for default state | spr-divide-mushroom-300 |
| `.spr-divide-color-hover` | Divider color on hover state | spr-divide-mushroom-400 |
| `.spr-divide-color-pressed` | Divider color on pressed state | spr-divide-mushroom-500 |
| `.spr-divide-color-weak` | Weak divider color for subtle separation | spr-divide-mushroom-200 |
| `.spr-divide-color-disabled` | Divider color for disabled elements | spr-divide-white-100 |
| `.spr-divide-color-on-fill-disabled` | Divider color on disabled filled backgrounds | spr-divide-white-200 |
| `.spr-divide-color-brand-base` | Brand divider color (base state) | spr-divide-kangkong-700 |
| `.spr-divide-color-brand-hover` | Brand divider color (hover state) | spr-divide-kangkong-800 |
| `.spr-divide-color-brand-pressed` | Brand divider color (pressed state) | spr-divide-kangkong-900 |
| `.spr-divide-color-success-base` | Success divider color (base state) | spr-divide-kangkong-700 |
| `.spr-divide-color-success-hover` | Success divider color (hover state) | spr-divide-kangkong-800 |
| `.spr-divide-color-success-pressed` | Success divider color (pressed state) | spr-divide-kangkong-900 |
| `.spr-divide-color-information-base` | Information divider color (base state) | spr-divide-blueberry-700 |
| `.spr-divide-color-information-hover` | Information divider color (hover state) | spr-divide-blueberry-800 |
| `.spr-divide-color-information-pressed` | Information divider color (pressed state) | spr-divide-blueberry-900 |
| `.spr-divide-color-danger-base` | Danger divider color (base state) | spr-divide-tomato-600 |
| `.spr-divide-color-danger-hover` | Danger divider color (hover state) | spr-divide-tomato-700 |
| `.spr-divide-color-danger-pressed` | Danger divider color (pressed state) | spr-divide-tomato-800 |
| `.spr-divide-color-pending-base` | Pending divider color (base state) | spr-divide-mango-700 |
| `.spr-divide-color-pending-hover` | Pending divider color (hover state) | spr-divide-mango-800 |
| `.spr-divide-color-pending-pressed` | Pending divider color (pressed state) | spr-divide-mango-900 |
| `.spr-divide-color-caution-base` | Caution divider color (base state) | spr-divide-carrot-700 |
| `.spr-divide-color-caution-hover` | Caution divider color (hover state) | spr-divide-carrot-800 |
| `.spr-divide-color-caution-pressed` | Caution divider color (pressed state) | spr-divide-carrot-900 |
| `.spr-divide-color-accent-base` | Accent divider color (base state) | spr-divide-wintermelon-700 |
| `.spr-divide-color-accent-hover` | Accent divider color (hover state) | spr-divide-wintermelon-800 |
| `.spr-divide-color-accent-pressed` | Accent divider color (pressed state) | spr-divide-wintermelon-900 |

---

## Box Shadows

Utility classes for consistent drop shadow effects to add depth and visual hierarchy.

### Drop Shadows

| Class | Description |
|---|---|
| `.spr-drop-shadow-sm` | Small drop shadow for subtle depth |
| `.spr-drop-shadow` | Medium drop shadow for standard elevation |
| `.spr-drop-shadow-md` | Large drop shadow for high elevation |

### Top Drop Shadows

| Class | Description |
|---|---|
| `.spr-drop-shadow-top-sm` | Small top drop shadow |
| `.spr-drop-shadow-top` | Medium top drop shadow |
| `.spr-drop-shadow-top-md` | Large top drop shadow |

### Usage Guidelines

- **Small shadows** (`spr-drop-shadow-sm`) — Cards, inputs, subtle elevation
- **Medium shadows** (`spr-drop-shadow`) — Buttons, modals, standard components
- **Large shadows** (`spr-drop-shadow-md`) — Dropdowns, tooltips, floating elements
- **Top shadows** — Sticky footers, bottom navigation

```html
<!-- Card with shadow -->
<div class="spr-background-color p-6 spr-rounded-border-radius-lg spr-drop-shadow">
  Card content
</div>

<!-- Footer with top shadow -->
<footer class="spr-drop-shadow-top p-4">Footer content</footer>
```

---

## Max Width

Additional max-width utility classes.

### Available Classes

| Max Width | Class |
|---|---|
| 640px | `spr-max-w-sm` |
| 1000px | `spr-max-w-md` |
| 1320px | `spr-max-w-lg` |

---

## Skeletal Loader

The Skeletal Loader is a CSS utility that provides placeholder elements to indicate loading content. It enhances user experience by showing animated placeholders during data fetch operations, reducing perceived loading times.

### Available Classes

| Class | Description |
|---|---|
| `spr-skeletal-loader` | Base skeletal loader class. Apply to a `div` and combine with sizing utilities. |

### Usage Example

Basic usage -- apply `spr-skeletal-loader` to a `div` along with standard Tailwind height and width utilities:

```html
<div class="space-y-2">
  <div class="spr-skeletal-loader h-4 w-full"></div>
  <div class="spr-skeletal-loader h-4 w-full"></div>
  <div class="spr-skeletal-loader h-4 w-full"></div>
</div>
```

### Variations

**Different sizes** -- control dimensions with standard Tailwind height/width utilities:

```html
<!-- Small loader -->
<div class="spr-skeletal-loader h-2 w-24"></div>

<!-- Medium loader -->
<div class="spr-skeletal-loader h-4 w-48"></div>

<!-- Large loader -->
<div class="spr-skeletal-loader h-6 w-full"></div>
```

**Rounded corners:**

```html
<div class="spr-skeletal-loader h-4 w-full spr-rounded-border-radius-md"></div>
```

**Circle loader** (for profile pictures or icons):

```html
<div class="spr-skeletal-loader h-12 w-12 rounded-full"></div>
```

**Complex card loading state:**

```html
<div class="space-y-4 p-4">
  <!-- Header -->
  <div class="flex items-center space-x-3">
    <div class="spr-skeletal-loader h-10 w-10 rounded-full"></div>
    <div class="flex-1 space-y-2">
      <div class="spr-skeletal-loader h-3 w-1/2"></div>
      <div class="spr-skeletal-loader h-2 w-1/4"></div>
    </div>
  </div>

  <!-- Content -->
  <div class="space-y-2">
    <div class="spr-skeletal-loader h-4 w-full"></div>
    <div class="spr-skeletal-loader h-4 w-full"></div>
    <div class="spr-skeletal-loader h-4 w-3/4"></div>
  </div>
</div>
```

---

## Miscellaneous

Additional utility classes for scrolling behavior and visual states.

### Hidden Scrollbars

| Class | Description |
|---|---|
| `.spr-hidden-scrolls` | Hides webkit scrollbars while maintaining scroll functionality |

Use when you want scrollable content but a cleaner appearance without visible scrollbars:

```html
<!-- Container with hidden scrollbar -->
<div class="spr-hidden-scrolls overflow-y-auto max-h-96">
  <!-- Scrollable content -->
</div>
```

**Best practices:**
- Use sparingly — only when design specifically calls for it
- Ensure accessibility — users should still understand content is scrollable
- Provide visual cues — consider fade effects or scroll indicators
