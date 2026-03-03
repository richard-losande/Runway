<template>
  <div class="flex flex-col items-center justify-center min-h-screen p-8">
    <div class="w-full max-w-lg text-center">
      <h1 class="text-3xl font-bold mb-2">Runway</h1>
      <p class="text-base text-gray-600 mb-8">
        Find out how long your savings would last — and what moves that number.
      </p>

      <div class="bg-white rounded-lg shadow p-6 mb-6 text-left">
        <div class="flex flex-col gap-6">
          <!-- Monthly Income (read-only) -->
          <div>
            <label class="block text-sm font-semibold text-gray-700 mb-1">Monthly Net Income</label>
            <input
              :value="'₱' + store.monthlyIncome.toLocaleString()"
              disabled
              class="w-full px-3 py-2 border border-gray-300 rounded-md bg-gray-100 text-gray-500"
            />
          </div>

          <!-- Fixed Deductions Summary -->
          <div class="p-3 bg-gray-50 rounded-md text-sm text-gray-600">
            <p class="font-semibold mb-1">Fixed Deductions</p>
            <p>SSS / PhilHealth / Pag-IBIG: ₱8,500</p>
            <p>Sprout Salary Loan: ₱5,000</p>
          </div>

          <!-- Liquid Savings -->
          <div>
            <label class="block text-sm font-semibold text-gray-700 mb-1">Liquid Savings (₱)</label>
            <input
              v-model.number="store.liquidSavings"
              type="number"
              placeholder="Enter your total savings"
              class="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-green-500"
            />
          </div>

          <!-- CSV Upload -->
          <div>
            <label class="block text-sm font-semibold text-gray-700 mb-2">Transaction History</label>
            <div class="flex gap-3">
              <button
                @click="store.useDemoData = true; store.csvFile = null"
                :class="[
                  'px-4 py-2 rounded-md text-sm font-medium border',
                  store.useDemoData
                    ? 'bg-green-50 border-green-300 text-green-700'
                    : 'bg-white border-gray-300 text-gray-700 hover:bg-gray-50'
                ]"
              >
                Use Demo Data
              </button>
              <input
                type="file"
                ref="fileInput"
                accept=".csv"
                class="hidden"
                @change="onFileSelected"
              />
              <button
                @click="(fileInput as HTMLInputElement)?.click()"
                class="px-4 py-2 rounded-md text-sm font-medium border border-gray-300 text-gray-700 hover:bg-gray-50"
              >
                Upload CSV
              </button>
            </div>
            <p v-if="store.csvFile" class="text-sm text-gray-500 mt-2">
              {{ store.csvFile.name }}
            </p>
          </div>

          <!-- Privacy Note -->
          <p class="text-sm text-gray-400 italic">
            Processed on your device. Nothing stored.
          </p>

          <!-- Error -->
          <p v-if="store.error" class="text-sm text-red-600">
            {{ store.error }}
          </p>

          <!-- CTA -->
          <button
            @click="onSubmit"
            :disabled="!isValid || store.isLoading"
            class="w-full py-3 px-4 rounded-md text-white font-semibold text-base disabled:opacity-50 disabled:cursor-not-allowed"
            :class="isValid ? 'bg-green-600 hover:bg-green-700' : 'bg-gray-400'"
          >
            {{ store.isLoading ? 'Analyzing...' : 'Show Me My Runway →' }}
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed } from 'vue'
import { useRunwayStore } from '../stores/runway'

const store = useRunwayStore()
const fileInput = ref<HTMLInputElement>()

const isValid = computed(() => store.liquidSavings > 0)

function onFileSelected(event: Event) {
  const target = event.target as HTMLInputElement
  if (target.files?.[0]) {
    store.csvFile = target.files[0]
    store.useDemoData = false
  }
}

function onSubmit() {
  store.analyze()
}
</script>
