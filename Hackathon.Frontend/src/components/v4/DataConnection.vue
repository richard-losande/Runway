<template>
  <div class="flex flex-col min-h-screen max-w-md mx-auto">
    <div class="flex-1 overflow-y-auto p-4 pb-24 space-y-5">
    <!-- Header -->
    <div>
      <h1 class="text-2xl font-bold text-gray-900">Choose how to analyze your spending</h1>
      <p class="text-sm text-gray-500 mt-1">
        Pick the method that works best for you. All three lead to your personalized runway report.
      </p>
    </div>

    <!-- Tier 1: GCash Connect (coming soon) -->
    <div
      class="w-full text-left bg-white rounded-2xl shadow-sm border-2 border-gray-200 overflow-hidden opacity-60 cursor-not-allowed"
    >
      <div class="p-5">
        <div class="flex items-start gap-3">
          <div class="w-10 h-10 rounded-xl bg-blue-500 flex items-center justify-center flex-shrink-0">
            <span class="text-white text-lg font-bold">G</span>
          </div>
          <div class="flex-1">
            <div class="flex items-center gap-2 mb-1">
              <h3 class="text-base font-semibold text-gray-900">GCash Connect</h3>
              <span class="inline-flex items-center px-2 py-0.5 rounded-full text-xs font-medium bg-gray-100 text-gray-500">
                Coming Soon
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
        </div>
      </div>
    </div>

    <!-- Tier 2: CSV Upload -->
    <div class="w-full text-left bg-white rounded-2xl shadow-sm border border-gray-200 hover:border-green-300 transition-colors overflow-hidden group">
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
            <!-- File input + drop zone -->
            <div
              class="mt-3 border-2 border-dashed rounded-lg p-3 text-center cursor-pointer transition-colors"
              :class="selectedFile ? 'border-green-400 bg-green-50' : 'border-gray-200 hover:border-green-300'"
              @click="($refs.fileInput as HTMLInputElement).click()"
              @dragover.prevent="dragOver = true"
              @dragleave.prevent="dragOver = false"
              @drop.prevent="onFileDrop"
            >
              <template v-if="selectedFile">
                <p class="text-sm font-medium text-green-700 truncate max-w-[250px]" :title="selectedFile.name">{{ selectedFile.name }}</p>
                <p class="text-xs text-green-600 mt-0.5">{{ (selectedFile.size / 1024).toFixed(1) }} KB</p>
              </template>
              <template v-else>
                <p class="text-xs text-gray-400">Drop CSV or Excel file here or tap to browse</p>
              </template>
            </div>
            <input
              ref="fileInput"
              type="file"
              accept=".csv,.xlsx,.xls"
              class="hidden"
              @change="onFileSelect"
            />
          </div>
        </div>
      </div>
    </div>

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
            <div class="mt-3">
              <spr-input
                :model-value="manualSpend"
                label="Estimated monthly spending"
                placeholder="20,000"
                @update:model-value="manualSpend = Number($event)"
              >
                <template #prefix>&#8369;</template>
              </spr-input>
            </div>
          </div>
        </div>
      </div>
    </div>

    </div>

    <!-- Fixed Footer -->
    <div class="fixed bottom-0 left-0 right-0 bg-white border-t border-gray-100 px-4 py-4 flex gap-3 rounded-b-2xl">
      <spr-button variant="secondary" size="large" fullwidth @click="store.goToScreen(2)">
        Back
      </spr-button>
      <spr-button v-if="selectedFile" tone="success" size="large" fullwidth @click="uploadCSV">
        Analyze This File
      </spr-button>
      <spr-button v-else tone="success" size="large" fullwidth @click="submitManual">
        Estimate
      </spr-button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useRunwayV4Store } from '../../stores/runway-v4'

const store = useRunwayV4Store()
const manualSpend = ref(20000)
const selectedFile = ref<File | null>(null)
const dragOver = ref(false)

function onFileSelect(event: Event) {
  const input = event.target as HTMLInputElement
  if (input.files && input.files.length > 0) {
    const file = input.files[0]
    if (file) selectedFile.value = file
  }
}

function onFileDrop(event: DragEvent) {
  dragOver.value = false
  if (event.dataTransfer?.files && event.dataTransfer.files.length > 0) {
    const file = event.dataTransfer.files[0]
    if (file && file.name.match(/\.(csv|xlsx|xls)$/i)) {
      selectedFile.value = file
    }
  }
}

function uploadCSV() {
  if (!selectedFile.value) return
  store.csvFile = selectedFile.value
  store.useDemoData = false
  store.analyze()
}

function submitManual() {
  if (store.payroll) {
    // Use payroll deductions as the expense breakdown
    store.analyzeFromPayroll(manualSpend.value)
  } else {
    store.useDemoData = true
    store.analyze()
  }
}
</script>
