import type { ZoneName } from '../api/runway-v4-types'

export const ZONE_CONFIG: Record<ZoneName, {
  colour: string
  colourToken: string
  label: string
  description: string
  product: 'ReadyWage' | 'ReadyCash' | 'ReadySave'
}> = {
  Critical: {
    colour: '#E53E3E',
    colourToken: 'TOMATO',
    label: 'Critical Zone',
    description: 'Savings cover less than a month. Focus on one action right now.',
    product: 'ReadyWage',
  },
  Fragile: {
    colour: '#DD6B20',
    colourToken: 'MANGO',
    label: 'Fragile Zone',
    description: 'About 1–2 months of runway. One unexpected expense could strain you.',
    product: 'ReadyCash',
  },
  Stable: {
    colour: '#3182CE',
    colourToken: 'BLUEBERRY',
    label: 'Stable Zone',
    description: '2–4 months of breathing room. Enough to handle most surprises — but not enough to stop watching.',
    product: 'ReadySave',
  },
  Strong: {
    colour: '#38A169',
    colourToken: 'KANGKONG',
    label: 'Strong Zone',
    description: '4+ months of cushion. Well-positioned — the goal now is to make this money work harder.',
    product: 'ReadySave',
  },
}

export const CATEGORY_LABELS: Record<string, string> = {
  FoodDining: 'Food & Dining',
  Groceries: 'Groceries & Market',
  BillsUtilities: 'Bills & Utilities',
  Transport: 'Transport',
  Shopping: 'Shopping',
  HealthWellness: 'Health & Wellness',
  Housing: 'Housing',
  Transfers: 'Transfers & Family',
  EntertainmentSubs: 'Entertainment & Subs',
  GovernmentDeductions: 'Government Deductions',
  Misc: 'Miscellaneous',
}
