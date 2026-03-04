<template>
  <div class="max-w-md mx-auto p-4 space-y-5">
    <!-- Header -->
    <div>
      <h1 class="text-2xl font-bold text-gray-900">Choose how to analyze your spending</h1>
      <p class="text-sm text-gray-500 mt-1">
        Pick the method that works best for you. All three lead to your personalized runway report.
      </p>
    </div>

    <!-- Tier 1: GCash Connect -->
    <button
      class="w-full text-left bg-white rounded-2xl shadow-sm border-2 border-green-200 hover:border-green-400 transition-colors overflow-hidden group"
      @click="connectGCash"
    >
      <div class="p-5">
        <div class="flex items-start gap-3">
          <div class="w-10 h-10 rounded-xl bg-blue-500 flex items-center justify-center flex-shrink-0">
            <span class="text-white text-lg font-bold">G</span>
          </div>
          <div class="flex-1">
            <div class="flex items-center gap-2 mb-1">
              <h3 class="text-base font-semibold text-gray-900">GCash Connect</h3>
              <span class="inline-flex items-center px-2 py-0.5 rounded-full text-xs font-medium bg-green-100 text-green-800">
                Recommended
              </span>
            </div>
            <div class="flex items-center gap-2 mb-2">
              <span class="inline-flex items-center px-2 py-0.5 rounded-full text-xs font-medium bg-emerald-100 text-emerald-800">
                Most accurate
              </span>
            </div>
            <p class="text-sm text-gray-500">
              Automatically pull 4 months of transaction data from your GCash account.
            </p>
          </div>
          <svg class="w-5 h-5 text-gray-400 group-hover:text-green-500 transition-colors mt-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7" />
          </svg>
        </div>
      </div>
    </button>

    <!-- Tier 2: CSV Upload -->
    <button
      class="w-full text-left bg-white rounded-2xl shadow-sm border border-gray-200 hover:border-green-300 transition-colors overflow-hidden group"
      @click="uploadCSV"
    >
      <div class="p-5">
        <div class="flex items-start gap-3">
          <div class="w-10 h-10 rounded-xl bg-gray-100 flex items-center justify-center flex-shrink-0">
            <svg class="w-5 h-5 text-gray-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                d="M7 16a4 4 0 01-.88-7.903A5 5 0 1115.9 6L16 6a5 5 0 011 9.9M15 13l-3-3m0 0l-3 3m3-3v12" />
            </svg>
          </div>
          <div class="flex-1">
            <h3 class="text-base font-semibold text-gray-900">CSV Upload</h3>
            <p class="text-sm text-gray-500 mt-1">
              Upload a bank or e-wallet statement file. We support most Philippine bank formats.
            </p>
            <!-- Drop zone hint -->
            <div class="mt-3 border-2 border-dashed border-gray-200 rounded-lg p-3 text-center">
              <p class="text-xs text-gray-400">Drop CSV file here or tap to browse</p>
            </div>
          </div>
          <svg class="w-5 h-5 text-gray-400 group-hover:text-green-500 transition-colors mt-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7" />
          </svg>
        </div>
      </div>
    </button>

    <!-- Tier 3: Manual Estimate -->
    <div class="bg-white rounded-2xl shadow-sm border border-gray-200 overflow-hidden">
      <div class="p-5">
        <div class="flex items-start gap-3">
          <div class="w-10 h-10 rounded-xl bg-gray-100 flex items-center justify-center flex-shrink-0">
            <svg class="w-5 h-5 text-gray-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z" />
            </svg>
          </div>
          <div class="flex-1">
            <h3 class="text-base font-semibold text-gray-900">Manual Estimate</h3>
            <p class="text-sm text-gray-500 mt-1">
              Enter your average monthly spending and we'll estimate from there.
            </p>

            <!-- Monthly Spend Input -->
            <div class="mt-3 space-y-1.5">
              <label class="text-xs font-medium text-gray-600">Estimated monthly spending</label>
              <div class="relative">
                <span class="absolute left-3 top-1/2 -translate-y-1/2 text-gray-500 font-medium text-sm">&#8369;</span>
                <input
                  v-model.number="manualSpend"
                  type="number"
                  placeholder="20,000"
                  class="w-full pl-8 pr-4 py-2.5 border border-gray-300 rounded-lg text-sm font-medium text-gray-900 focus:ring-2 focus:ring-green-500 focus:border-green-500 outline-none"
                />
              </div>
            </div>
          </div>
        </div>
      </div>
      <div class="px-5 pb-4">
        <button
          class="w-full py-2.5 px-4 bg-gray-100 hover:bg-gray-200 text-gray-700 font-medium rounded-lg transition-colors text-sm"
          @click="submitManual"
        >
          Use Estimate
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useRunwayV4Store } from '../../stores/runway-v4'

const store = useRunwayV4Store()
const manualSpend = ref(20000)

function connectGCash() {
  store.useDemoData = true
  store.analyze()
}

function uploadCSV() {
  store.useDemoData = true
  store.analyze()
}

function submitManual() {
  store.useDemoData = true
  store.analyze()
}
</script>
