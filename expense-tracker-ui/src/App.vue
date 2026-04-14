<script setup>
import { ref, onMounted } from 'vue'
import { subscriptionsApi } from './api.js'
import SubscriptionForm from './components/SubscriptionForm.vue'
import SubscriptionList from './components/SubscriptionList.vue'

const subscriptions = ref([])
const summary = ref(null)
const editingItem = ref(null)
const showForm = ref(false)
const loading = ref(false)
const error = ref(null)

async function fetchAll() {
  loading.value = true
  error.value = null
  try {
    const [subRes, sumRes] = await Promise.all([
      subscriptionsApi.getAll(),
      subscriptionsApi.getSummary()
    ])
    subscriptions.value = subRes.data
    summary.value = sumRes.data
  } catch (e) {
    error.value = 'Nu pot contacta API-ul. Asigură-te că backend-ul rulează pe portul 5000.'
  } finally {
    loading.value = false
  }
}

async function handleSubmit(payload) {
  if (editingItem.value) {
    await subscriptionsApi.update(editingItem.value.id, payload)
    editingItem.value = null
    showForm.value = false
  } else {
    await subscriptionsApi.create(payload)
    showForm.value = false
  }
  await fetchAll()
}

async function handleDelete(id) {
  if (!confirm('Sigur ștergi acest abonament?')) return
  await subscriptionsApi.remove(id)
  await fetchAll()
}

function handleEdit(item) {
  editingItem.value = item
  showForm.value = true
  window.scrollTo({ top: 0, behavior: 'smooth' })
}

function handleCancel() {
  editingItem.value = null
  showForm.value = false
}

onMounted(fetchAll)
</script>

<template>
  <div class="min-h-screen bg-gray-950 text-white">
    <!-- Nav -->
    <header class="border-b border-gray-800 bg-gray-950/80 backdrop-blur sticky top-0 z-10">
      <div class="max-w-6xl mx-auto px-4 py-4 flex items-center justify-between">
        <div class="flex items-center gap-3">
          <div class="w-8 h-8 bg-indigo-600 rounded-lg flex items-center justify-center text-sm font-bold">$</div>
          <span class="font-semibold text-white">Expense Tracker</span>
        </div>
        <button
          @click="showForm = !showForm; editingItem = null"
          class="bg-indigo-600 hover:bg-indigo-500 text-white text-sm font-medium px-4 py-2 rounded-lg transition"
        >
          {{ showForm && !editingItem ? '✕ Închide' : '+ Abonament nou' }}
        </button>
      </div>
    </header>

    <main class="max-w-6xl mx-auto px-4 py-8 space-y-8">

      <!-- Error -->
      <div v-if="error" class="bg-red-500/10 border border-red-500/30 text-red-400 rounded-xl p-4 text-sm">
        {{ error }}
      </div>

      <!-- Summary Cards -->
      <div v-if="summary" class="grid grid-cols-2 sm:grid-cols-4 gap-4">
        <div class="bg-gray-900 border border-gray-800 rounded-2xl p-4">
          <p class="text-xs text-gray-500 mb-1">Total lunar</p>
          <p class="text-2xl font-bold text-indigo-400">
            {{ summary.totalMonthlyEquivalent.toFixed(2) }}
            <span class="text-sm font-normal text-gray-500">
              {{ summary.byCurrency?.[0]?.currency ?? '' }}
            </span>
          </p>
        </div>
        <div class="bg-gray-900 border border-gray-800 rounded-2xl p-4">
          <p class="text-xs text-gray-500 mb-1">Total anual</p>
          <p class="text-2xl font-bold text-white">
            {{ (summary.totalMonthlyEquivalent * 12).toFixed(2) }}
            <span class="text-sm font-normal text-gray-500">
              {{ summary.byCurrency?.[0]?.currency ?? '' }}
            </span>
          </p>
        </div>
        <div class="bg-gray-900 border border-gray-800 rounded-2xl p-4">
          <p class="text-xs text-gray-500 mb-1">Abonamente active</p>
          <p class="text-2xl font-bold text-emerald-400">{{ summary.activeSubscriptions }}</p>
        </div>
        <div class="bg-gray-900 border border-gray-800 rounded-2xl p-4">
          <p class="text-xs text-gray-500 mb-1">Total abonamente</p>
          <p class="text-2xl font-bold text-white">{{ summary.totalSubscriptions }}</p>
        </div>
      </div>

      <!-- Form (toggle) -->
      <Transition name="slide">
        <SubscriptionForm
          v-if="showForm || editingItem"
          :editing-item="editingItem"
          @submit="handleSubmit"
          @cancel="handleCancel"
        />
      </Transition>

      <!-- List -->
      <div>
        <div class="flex items-center justify-between mb-4">
          <h2 class="text-sm font-medium text-gray-400 uppercase tracking-wider">Abonamente</h2>
          <button
            v-if="!loading"
            @click="fetchAll"
            class="text-xs text-gray-500 hover:text-gray-300 transition"
          >
            ↻ Reîncarcă
          </button>
        </div>

        <div v-if="loading" class="text-center text-gray-600 py-16 animate-pulse">
          Se încarcă...
        </div>

        <SubscriptionList
          v-else
          :subscriptions="subscriptions"
          @edit="handleEdit"
          @delete="handleDelete"
        />
      </div>
    </main>
  </div>
</template>

<style>
.slide-enter-active,
.slide-leave-active {
  transition: all 0.2s ease;
}
.slide-enter-from,
.slide-leave-to {
  opacity: 0;
  transform: translateY(-8px);
}
</style>
