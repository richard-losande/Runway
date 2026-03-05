<template>
  <spr-card :show-footer="isOpen" :has-content-padding="false">
    <!-- Header -->
    <template #header>
      <div class="flex items-center justify-between w-full p-4">
        <div class="flex items-center gap-2">
          <Icon icon="ph:receipt" style="width:24px;height:24px;color:#158039;" />
          <span class="spr-text-color-strong" style="font-weight: 600;">Payslip</span>
          <span class="inline-block w-2 h-2 rounded-full bg-green-500 ml-1" />
        </div>
        <spr-button variant="secondary" size="small" hasIcon @click="isOpen = !isOpen">
          <Icon :icon="isOpen ? 'ph:caret-up' : 'ph:caret-down'" />
        </spr-button>
      </div>
    </template>

    <!-- Content -->
    <template #content>
      <div v-if="isOpen">
        <!-- Latest Transactions header -->
        <div class="flex items-center justify-between px-4 pt-3 pb-1">
          <span class="spr-label-sm-medium spr-text-color-supporting">LATEST TRANSACTIONS</span>
          <spr-button variant="secondary" size="small" hasIcon @click="showAmounts = !showAmounts">
            <Icon :icon="showAmounts ? 'ph:eye' : 'ph:eye-slash'" />
          </spr-button>
        </div>

        <!-- Transaction rows -->
        <div class="px-2 pb-2">
          <div
            v-for="(tx, index) in transactions"
            :key="tx.period"
            class="tx-row flex items-center justify-between py-2.5 cursor-pointer rounded-lg px-3 transition-colors group"
            @click="openPayslip(tx)"
          >
            <div class="flex flex-col">
              <div class="flex items-center gap-1.5">
                <span class="spr-body-sm-regular-medium spr-text-color-strong" style="font-weight: 600;">{{ tx.type }}</span>
                <span v-if="index === 0" class="inline-block w-1.5 h-1.5 rounded-full bg-green-500" />
              </div>
              <span class="spr-body-sm-regular spr-text-color-supporting">{{ tx.period }}</span>
            </div>
            <div class="flex items-center gap-2">
              <div class="flex items-center gap-1">
                <span class="spr-body-sm-semibold spr-text-color-strong">{{ showAmounts ? tx.amount : '******' }}</span>
                <span class="spr-body-sm-regular spr-text-color-supporting">{{ tx.currency }}</span>
              </div>
              <Icon icon="ph:caret-right" class="caret-icon spr-text-color-supporting" style="opacity:0; transition: opacity 0.15s;" />
            </div>
          </div>
        </div>
      </div>
    </template>

    <!-- Footer -->
    <template #footer>
      <div class="flex items-center justify-end gap-2 w-full">
        <spr-button variant="secondary" size="small" hasIcon>
          <Icon icon="ph:download-simple" />
          <span>Download BIR 2316</span>
        </spr-button>
        <spr-button variant="secondary" size="small" hasIcon>
          <Icon icon="ph:table" />
          <span>View Payroll Summary</span>
        </spr-button>
      </div>
    </template>
  </spr-card>

  <!-- Stacking Sidepanel -->
  <spr-stacking-sidepanel
    ref="stackingRef"
    v-model:stack="stack"
  >
    <!-- Payslip Panel -->
    <template #payslip>
      <spr-sidepanel
        :is-open="true"
        :is-stacking="true"
        :is-active-panel="stack.at(-1) === 'payslip'"
        :header-title="selectedTx?.type ?? 'Payslip'"
        size="lg"
        @close="stackingRef?.hidePanel('payslip')"
      >
        <template #default>
          <div class="p-4">
            <!-- Payslip document -->
            <div class="bg-white border rounded-xl overflow-hidden" style="border-color: #e5e7eb;">
              <!-- Document header -->
              <div class="px-4 py-4 border-b" style="border-color: #e5e7eb; background: #f9fafb;">
                <!-- Sprout logo -->
                <div class="mb-4">
                  <svg height="28" viewBox="0 0 3790 1292" fill="none" xmlns="http://www.w3.org/2000/svg">
                    <path d="M1376.26 855.383C1263.41 855.383 1204.69 795.745 1203.77 710.416H1279.93C1280.84 755.374 1305.62 785.652 1380.85 785.652H1421.22C1492.79 785.652 1522.15 761.797 1522.15 721.426C1522.15 671.881 1487.28 657.201 1431.32 648.025C1417.55 645.273 1369.84 636.098 1353.33 632.428C1271.67 617.747 1211.11 591.14 1211.11 495.718C1211.11 414.06 1271.67 365.431 1381.77 365.431H1423.06C1534.08 365.431 1591.88 425.987 1591.88 505.811H1514.81C1514.81 465.44 1491.87 434.245 1418.47 434.245H1386.36C1316.63 434.245 1288.18 458.1 1288.18 497.553C1288.18 543.429 1321.21 558.109 1372.59 567.284C1388.19 570.037 1436.82 580.129 1453.34 582.882C1534.08 596.645 1599.22 624.17 1599.22 721.426C1599.22 805.838 1537.75 855.383 1424.89 855.383H1376.26Z" fill="#092903"/>
                    <path d="M1843.67 854.466C1791.37 854.466 1751.91 837.033 1726.22 807.673L1727.14 833.363V1033.38H1650.99V557.192C1650.99 436.997 1719.8 364.514 1843.67 364.514H1874.86C1996.89 364.514 2068.46 437.915 2068.46 557.192V661.788C2068.46 781.982 1996.89 854.466 1885.87 854.466H1843.67ZM1873.94 781.065C1947.34 781.065 1991.39 735.189 1991.39 660.871V559.027C1991.39 483.791 1947.34 438.833 1873.94 438.833H1844.58C1770.26 438.833 1727.14 483.791 1727.14 559.027V660.871C1727.14 735.189 1770.26 781.065 1844.58 781.065H1873.94Z" fill="#092903"/>
                    <path d="M2394.98 376.442V449.843H2328C2259.18 449.843 2210.56 500.306 2210.56 571.872V843.456H2134.4V563.614C2134.4 446.173 2208.72 376.442 2326.16 376.442H2394.98Z" fill="#092903"/>
                    <path d="M2598.8 855.383C2476.77 855.383 2405.21 781.982 2405.21 662.706V558.109C2405.21 437.915 2476.77 365.431 2598.8 365.431H2629.08C2750.19 365.431 2821.76 437.915 2821.76 558.109V662.706C2821.76 781.982 2750.19 855.383 2629.08 855.383H2598.8ZM2627.24 781.065C2700.64 781.065 2744.68 736.107 2744.68 660.871V559.027C2744.68 484.708 2700.64 438.833 2627.24 438.833H2599.72C2526.32 438.833 2482.28 484.708 2482.28 559.027V660.871C2482.28 736.107 2526.32 781.065 2599.72 781.065H2627.24Z" fill="#092903"/>
                    <path d="M3078.8 855.383C2958.61 855.383 2888.88 784.735 2888.88 665.458V376.442H2965.03V664.541C2965.03 737.024 3008.15 781.065 3077.89 781.065H3100.82C3170.55 781.065 3214.59 737.024 3214.59 664.541V376.442H3290.75V665.458C3290.75 784.735 3220.1 855.383 3100.82 855.383H3078.8Z" fill="#092903"/>
                    <path d="M3367.81 259H3443.97V376.442H3598.11V447.09H3443.97V659.036C3443.97 738.859 3497.18 771.89 3565.08 771.89H3610.95V843.456H3554.07C3442.13 843.456 3367.81 781.065 3367.81 665.458V259Z" fill="#092903"/>
                    <g clip-path="url(#clip0_payslip_logo)">
                      <path d="M495.669 191.025C559.997 180.851 612.839 190.506 642.651 198.274C660.819 203.008 733.499 223.455 801.532 286.896L805.677 290.818C847.814 331.329 869.32 370.904 875.117 381.96L875.121 381.963C899.478 428.433 907.02 469.042 910.644 489.737L910.661 489.786L910.744 490.269C913.524 506.209 917.234 528.046 916.173 556.925C915.094 586.603 910.306 609.121 907.19 622.772C904.449 634.805 901.409 647.86 894.616 664.381L894.613 664.384C893.043 668.195 887.011 682.563 876.292 699.889C870.299 709.578 864.661 717.239 860.503 722.576L860.507 722.579C857.042 727.03 853.379 731.876 847.95 737.836C846.403 739.538 839.458 747.1 828.803 756.184H828.799C826.881 757.819 818.406 764.98 805.846 773.425L803.281 775.129C790.432 783.567 779.551 789.176 773.572 792.114C769.191 794.26 754.408 801.355 733.814 807.879C710.184 815.365 690.867 818.243 681.596 819.404C665.694 821.388 653.539 821.547 645.873 821.656H645.867C634.491 821.802 616.519 821.474 594.657 818.482L594.654 818.479C574.528 815.71 557.867 811.617 545.881 808.157C508.421 797.338 479.89 782.041 462.569 771.498C445.681 761.216 421.33 744.38 396.257 718.951C361.226 683.422 341.563 648.099 331.817 628.145V628.142C315.337 594.424 306.803 563.011 302.288 538.05L301.436 533.148C295.238 495.856 296.218 463.875 298.607 440.835L299.106 436.022L303.847 435.028C330.189 429.508 365.492 424.269 407.294 424.135C481.5 423.891 538.7 439.86 567.112 449.218C588.041 456.103 621.769 467.434 660.989 491.577L664.801 493.955L664.805 493.958C671.75 498.348 701.596 517.486 734.869 549.745H734.873C759.87 573.986 775.901 595.081 785.04 607.207L786.938 609.744C791.245 615.555 794.653 620.504 797.086 624.046L798.333 625.893C804.283 634.825 806.138 639.701 816.475 657.654C817.822 659.989 819.03 662.049 820.102 663.876C826.47 657.037 830.621 649.843 834.179 643.488L834.182 643.482C841.067 631.223 844.84 619.877 847.429 609.96C849.367 602.51 858.001 569.466 852.599 520.743L851.932 514.919C850.183 500.3 846.908 478.55 837.978 452.343C818.264 394.505 785.355 357.117 772.563 343.594H772.559C762.296 332.737 715.805 285.278 637.242 261.865C576.396 243.735 525.685 248.758 505.29 251.636C460.105 258.006 426.776 273.041 411.546 280.738L408.743 282.176C342.451 316.625 306.652 367.593 292.81 389.831C253.231 453.424 248.848 513.99 246.553 552.541C242.882 614.222 254.502 660.304 264.772 700.994C279.728 760.264 300.207 804.701 324.286 853.046L334.821 874.057C345.092 894.488 365.613 933.798 395.226 980.332L400.589 988.667C408.301 1000.53 422.786 1022.33 441.638 1048.38V1048.38C454.544 1066.23 466.372 1081.73 476.504 1094.64L484.926 1105.37H416.387L414.397 1102.69C412.503 1100.14 410.536 1097.46 408.48 1094.64L402.031 1085.74C386.297 1063.99 378.285 1052.93 376.758 1050.68V1050.68C356.105 1020.43 341.531 995.791 334.393 983.556C300.538 925.536 280.683 891.527 260.251 846.621C239.116 800.181 213.311 743.521 199.626 664.447L198.378 657.187C192.074 620.106 185.14 571.104 193.449 508.896C198.762 469.086 207.838 406.299 252.483 341.401C296.071 278.029 350.183 247.04 373.955 233.757C427.955 203.579 475.695 194.354 492.597 191.524L495.669 191.025Z" fill="#32CE13" stroke="#32CE13" stroke-width="13.3274" stroke-miterlimit="10"/>
                    </g>
                    <defs>
                      <clipPath id="clip0_payslip_logo">
                        <rect width="740" height="932" fill="white" transform="translate(183 180)"/>
                      </clipPath>
                    </defs>
                  </svg>
                </div>
                <div class="flex items-start justify-between">
                  <div class="flex flex-col gap-1">
                    <span class="text-xs font-semibold spr-text-color-supporting tracking-widest uppercase">Payslip</span>
                    <span class="text-lg font-bold spr-text-color-strong">Sprout Solutions</span>
                    <span class="text-sm spr-text-color-supporting">123 Galleria Corporate Center, Ortigas</span>
                  </div>
                  <div class="flex flex-col items-end gap-1">
                    <span class="text-xs spr-text-color-supporting">Pay Period</span>
                    <span class="text-sm font-semibold spr-text-color-strong">{{ selectedTx?.period }}</span>
                    <span class="text-xs spr-text-color-supporting mt-1">Pay Date</span>
                    <span class="text-sm font-semibold spr-text-color-strong">Dec 31, 2025</span>
                  </div>
                </div>
              </div>

              <!-- Employee info -->
              <div class="px-4 py-4 border-b grid grid-cols-2 gap-4" style="border-color: #e5e7eb;">
                <div class="flex flex-col gap-0.5">
                  <span class="text-xs spr-text-color-supporting">Employee</span>
                  <span class="text-sm font-semibold spr-text-color-strong">Jane Doe</span>
                </div>
                <div class="flex flex-col gap-0.5">
                  <span class="text-xs spr-text-color-supporting">Employee ID</span>
                  <span class="text-sm font-semibold spr-text-color-strong">EMP-00142</span>
                </div>
                <div class="flex flex-col gap-0.5">
                  <span class="text-xs spr-text-color-supporting">Department</span>
                  <span class="text-sm font-semibold spr-text-color-strong">Product Design</span>
                </div>
                <div class="flex flex-col gap-0.5">
                  <span class="text-xs spr-text-color-supporting">Position</span>
                  <span class="text-sm font-semibold spr-text-color-strong">Senior UX Designer</span>
                </div>
              </div>

              <!-- Earnings -->
              <div class="px-4 py-4 border-b" style="border-color: #e5e7eb;">
                <span class="text-xs font-semibold spr-text-color-supporting tracking-widest uppercase block mb-3">Earnings</span>
                <div class="flex flex-col gap-2">
                  <div v-for="item in earnings" :key="item.label" class="flex items-center justify-between">
                    <span class="text-sm spr-text-color-supporting">{{ item.label }}</span>
                    <span class="text-sm font-medium spr-text-color-strong">₱{{ item.amount }}</span>
                  </div>
                </div>
                <div class="flex items-center justify-between mt-3 pt-3 border-t" style="border-color: #e5e7eb;">
                  <span class="text-sm font-semibold spr-text-color-strong">Gross Pay</span>
                  <span class="text-sm font-semibold spr-text-color-strong">₱75,000.00</span>
                </div>
              </div>

              <!-- Deductions -->
              <div class="px-4 py-4 border-b" style="border-color: #e5e7eb;">
                <span class="text-xs font-semibold spr-text-color-supporting tracking-widest uppercase block mb-3">Deductions</span>
                <div class="flex flex-col gap-2">
                  <div v-for="item in deductions" :key="item.label" class="flex items-center justify-between">
                    <span class="text-sm spr-text-color-supporting">{{ item.label }}</span>
                    <span class="text-sm font-medium" style="color: #ef4444;">-₱{{ item.amount }}</span>
                  </div>
                </div>
                <div class="flex items-center justify-between mt-3 pt-3 border-t" style="border-color: #e5e7eb;">
                  <span class="text-sm font-semibold spr-text-color-strong">Total Deductions</span>
                  <span class="text-sm font-semibold" style="color: #ef4444;">-₱9,750.00</span>
                </div>
              </div>

              <!-- Net Pay -->
              <div class="px-4 py-4 rounded-b-xl" style="background: rgb(249, 250, 251);">
                <div class="flex items-center justify-between">
                  <div class="flex flex-col gap-0.5">
                    <span class="text-xs font-semibold spr-text-color-supporting tracking-widest uppercase">Net Pay</span>
                    <span class="text-xs spr-text-color-supporting">After all deductions</span>
                  </div>
                  <span class="text-xl font-bold" style="color: #158039;">₱65,250.00</span>
                </div>
              </div>
            </div>
          </div>
        </template>
        <template #footer>
          <div class="w-full flex flex-col gap-3 px-4 py-3">
            <!-- Runway card with animated gradient -->
            <div class="runway-gradient-card flex items-center justify-between gap-4 rounded-lg px-4 py-3">
              <div class="flex flex-col gap-0.5">
                <div class="flex items-center gap-2">
                  <span class="spr-body-md-semibold spr-text-color-strong" style="font-weight: 600;">Runway</span>
                  <span class="rounded-full px-2 py-0.5 text-xs font-semibold" style="background: #2563eb; color: #fff; white-space: nowrap;">Try Now!</span>
                </div>
                <span class="spr-body-sm-regular spr-text-color-supporting">Income sustainability, based on payroll.</span>
              </div>
              <spr-button variant="tertiary" size="small" hasIcon @click="openRunway()">
                <Icon icon="ph:arrow-right" />
              </spr-button>
            </div>
            <!-- Download PDF full width -->
            <spr-button tone="success" size="medium" fullwidth hasIcon>
              <Icon icon="ph:download-simple" />
              <span>Download Payslip PDF</span>
            </spr-button>
          </div>
        </template>
      </spr-sidepanel>
    </template>

    <!-- Runway Panel: Screen 2 — PayrollProfile -->
    <template #runway-2>
      <spr-sidepanel
        :is-open="true"
        :is-stacking="true"
        :is-active-panel="stack.at(-1) === 'runway-2'"
        header-title="Runway"
        header-subtitle="Income sustainability, based on payroll."
        size="lg"
        @close="closeRunwayPanels()"
      >
        <template #default>
          <PayrollProfile />
        </template>
      </spr-sidepanel>
    </template>

    <!-- Runway Panel: Screen 3 — DataConnection -->
    <template #runway-3>
      <spr-sidepanel
        :is-open="true"
        :is-stacking="true"
        :is-active-panel="stack.at(-1) === 'runway-3'"
        header-title="Runway"
        header-subtitle="Income sustainability, based on payroll."
        size="lg"
        @close="stackingRef?.hidePanel('runway-3')"
      >
        <template #default>
          <DataConnection />
        </template>
      </spr-sidepanel>
    </template>

    <!-- Runway Panel: Screens 4–5 — Processing + Intelligence Report -->
    <template #runway-4>
      <spr-sidepanel
        :is-open="true"
        :is-stacking="true"
        :is-active-panel="stack.at(-1) === 'runway-4'"
        header-title="Runway"
        header-subtitle="Income sustainability, based on payroll."
        size="lg"
        @close="stackingRef?.hidePanel('runway-4')"
      >
        <template #default>
          <ProcessingScreenV4   v-if="store.currentScreen === 4" />
          <IntelligenceReportV4 v-else-if="store.currentScreen === 5" />
        </template>
      </spr-sidepanel>
    </template>

    <!-- Runway Panel: Screens 6–8 — Survival Dashboard + Diagnosis + Action -->
    <template #runway-5>
      <spr-sidepanel
        :is-open="true"
        :is-stacking="true"
        :is-active-panel="stack.at(-1) === 'runway-5'"
        header-title="Runway"
        header-subtitle="Income sustainability, based on payroll."
        size="lg"
        @close="stackingRef?.hidePanel('runway-5')"
      >
        <template #default>
          <SurvivalDashboardV4 v-if="store.currentScreen === 6" />
          <DiagnosisScreenV4   v-else-if="store.currentScreen === 7" />
          <ActionCard          v-else-if="store.currentScreen === 8" />
        </template>
      </spr-sidepanel>
    </template>
  </spr-stacking-sidepanel>
</template>

<style scoped>
:deep(#headers) {
  flex: 1;
  min-width: 0;
}
:deep(#sidepanel-title) {
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}
:deep(#sidepanel-subtitle),
:deep(#sidepanel-subtitle span) {
  max-width: 100% !important;
  white-space: nowrap !important;
  word-break: keep-all !important;
  overflow-wrap: normal !important;
}
.runway-gradient-card {
  background: linear-gradient(120deg, #C9F3BE, #ffffff, #D2F612, #C9F3BE);
  background-size: 300% 300%;
  animation: runway-gradient 4s ease infinite;
}
@keyframes runway-gradient {
  0% { background-position: 0% 50%; }
  50% { background-position: 100% 50%; }
  100% { background-position: 0% 50%; }
}
.tx-row:hover {
  background-color: #eff1f1;
}
.tx-row:hover .caret-icon {
  opacity: 1 !important;
}
</style>

<script setup lang="ts">
import { ref, watch } from 'vue'
import { Icon } from '@iconify/vue'
import { useRunwayV4Store } from '../../stores/runway-v4'
import PayrollProfile from '../../components/v4/PayrollProfile.vue'
import DataConnection from '../../components/v4/DataConnection.vue'
import ProcessingScreenV4 from '../../components/v4/ProcessingScreenV4.vue'
import IntelligenceReportV4 from '../../components/v4/IntelligenceReportV4.vue'
import SurvivalDashboardV4 from '../../components/v4/SurvivalDashboardV4.vue'
import DiagnosisScreenV4 from '../../components/v4/DiagnosisScreenV4.vue'
import ActionCard from '../../components/v4/ActionCard.vue'

const store = useRunwayV4Store()

const isOpen = ref(true)
const showAmounts = ref(false)
const selectedTx = ref<(typeof transactions)[number] | null>(null)
const stack = ref<string[]>([])
const stackingRef = ref<{ showPanel: (id: string) => void; hidePanel: (id: string) => void } | null>(null)

// Watch store screen changes to manage the stacking panel stack
watch(() => store.currentScreen, (newScreen, oldScreen) => {
  if (!stack.value.includes('runway-2')) return

  // Forward transitions — push new panel
  if (oldScreen === 2 && newScreen === 3) {
    stackingRef.value?.showPanel('runway-3')       // Continue on PayrollProfile
  } else if (oldScreen === 3 && newScreen === 4) {
    stackingRef.value?.showPanel('runway-4')       // Estimate on DataConnection
  } else if (oldScreen === 5 && newScreen === 6) {
    stackingRef.value?.showPanel('runway-5')       // Get My Breakdown on IntelligenceReport
  }
  // Backward transitions — pop panel
  else if (oldScreen === 3 && newScreen === 2) {
    stackingRef.value?.hidePanel('runway-3')       // Back on DataConnection
  } else if (newScreen === 3 && stack.value.includes('runway-4')) {
    stackingRef.value?.hidePanel('runway-4')       // Back on IntelligenceReport
  } else if (newScreen === 5 && stack.value.includes('runway-5')) {
    stackingRef.value?.hidePanel('runway-5')       // Back on SurvivalDashboard
  } else if (newScreen === 1) {
    closeRunwayPanels()                            // Back on PayrollProfile
  }
})

function openRunway() {
  const grossPay = 75000
  const totalDeductions = deductions.reduce((sum, d) => sum + parseFloat(d.amount.replace(/,/g, '')), 0)
  const netPay = grossPay - totalDeductions
  store.payroll = {
    grossPay,
    netPay,
    tax: 7487.5,
    payrollPeriod: selectedTx.value?.period ?? 'Dec 16 - 31, 2025',
    employeeName: 'Jane Doe',
    earnings: earnings.map(e => ({ name: e.label, amount: parseFloat(e.amount.replace(/,/g, '')) })),
    deductions: deductions.map(d => ({ name: d.label, amount: parseFloat(d.amount.replace(/,/g, '')) })),
  }
  store.monthlyIncome = netPay
  store.goToScreen(2)
  stackingRef.value?.showPanel('runway-2')
}

function closeRunwayPanels() {
  ;['runway-5', 'runway-4', 'runway-3', 'runway-2']
    .filter(p => stack.value.includes(p))
    .forEach(p => stackingRef.value?.hidePanel(p))
}

const transactions = [
  { type: 'Payroll', period: 'Dec\u00A016\u00A0-\u00A031,\u00A02025', amount: '₱75,000.00', currency: 'PHP' },
  { type: 'Payroll', period: 'Dec\u00A001\u00A0-\u00A015,\u00A02025', amount: '₱75,000.00', currency: 'PHP' },
  { type: '13th Month Pay', period: '2024', amount: '₱75,000.00', currency: 'PHP' },
]

const earnings = [
  { label: 'Basic Salary', amount: '37,500.00' },
  { label: 'Transportation Allowance', amount: '2,000.00' },
  { label: 'Rice Subsidy', amount: '1,500.00' },
  { label: 'Overtime Pay', amount: '34,000.00' },
]

const deductions = [
  { label: 'SSS Contribution', amount: '1,125.00' },
  { label: 'PhilHealth', amount: '937.50' },
  { label: 'Pag-IBIG', amount: '200.00' },
  { label: 'Withholding Tax', amount: '7,487.50' },
]

function openPayslip(tx: (typeof transactions)[number]) {
  selectedTx.value = tx
  stackingRef.value?.showPanel('payslip')
}
</script>
