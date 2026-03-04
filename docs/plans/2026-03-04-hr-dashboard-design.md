# HR Dashboard Design

## Goal
Add a new desktop HR dashboard page to the existing Vue 3 frontend, matching the Figma design at node 111:790 ("Dashboard base"). The page lives alongside the existing financial runway screens without modifying them.

## Decisions
- **Target**: Hackathon.Frontend (Vue 3 SPA)
- **Data**: Static/mock data matching Figma placeholders
- **Navigation**: Vue Router — `/` for existing runway flow, `/dashboard` for new page
- **Components**: Sprout Design System (design-system-next v2.27.9, already installed and globally registered)
- **Architecture**: Organized sub-components in `views/dashboard/` folder

## Layout (from Figma)
```
1366x768 desktop frame
┌──────┬──────────────────────────────────────────┐
│ Side │  Announcement Bar (56px)                  │
│ Nav  ├───────────────────────────────────────────┤
│ 68px │  Page Header: "Dashboard" + Customize btn │
│      │  Profile Card (88px): greeting, clock     │
│      │  ┌──────────────────┬─────────────────┐   │
│      │  │ My Tasks (312px) │ Payslips        │   │
│      │  │                  │ widget          │   │
│      │  │ Leave Balance    │                 │   │
│      │  └──────────────────┴─────────────────┘   │
└──────┴───────────────────────────────────────────┘
```

## File Structure
```
src/
  views/
    dashboard/
      DashboardPage.vue      # Page shell: sidebar + main content
      DashboardSidebar.vue   # 68px collapsed icon rail
      AnnouncementBar.vue    # Green banner with announcements
      ProfileCard.vue        # Greeting, shift info, clock in/out
      TasksWidget.vue        # "My Tasks" card with task rows
      PayslipsWidget.vue     # Payslips card with transactions
      LeaveWidget.vue        # Leave credits tabs + leave type rows
    RunwayApp.vue            # Extracted from current App.vue
  router.ts                  # Vue Router config
```

## Component → Sprout DS Mapping
| Widget | Sprout Components Used |
|--------|----------------------|
| Sidebar | Icon, Logo, Badge, Avatar |
| Announcement | Icon, Button |
| Profile Card | Avatar, Badge, Lozenge, Button |
| Tasks Widget | Card, Badge, Lozenge, Button |
| Payslips Widget | Card, Badge, Button |
| Leave Widget | Card, Tabs, Badge |

## Design Tokens (from Figma)
- **Font**: Rubik (already loaded by design-system-next)
- **Colors**: primary green #158039, accent green #17ad49, dark #00291b, text #262b2b, gray #5d6c6b/#738482, bg #f1f2f3/#f7f8f8, red #da2f38, orange #cc5c02, blue #0f6eeb, light green #dcfce6
- **Spacing**: 8/12/16/24/80px padding, 8/16px gaps
- **Radii**: 6/8/12/16/32px

## Mock Data
- **Profile**: "Hello, Jane", shift 09:00 AM–06:00 PM, status "Expected at 09:00 AM"
- **Announcement**: "Happy Birthday to these Rockstars!"
- **Tasks** (5 items): Missing Log (Needs resolution), IT Acknowledgement Form (Needs action), etc.
- **Payslips** (3 items): Dec 16-31 Payroll, Dec 1-15 Payroll, 13th Month Pay 2024 — all "******" PHP
- **Leave** (9 types): Vacation, Sick, Emergency, Magna Carta, Paternity, Service Incentive, Bereavement, Comprehensive Family Care — each 20/40 days
- **Tabs**: This Pay Period, This Month, Year-To-Date
