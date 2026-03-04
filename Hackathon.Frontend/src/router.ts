import { createRouter, createWebHistory } from 'vue-router'

const router = createRouter({
  history: createWebHistory(),
  routes: [
    {
      path: '/',
      component: () => import('./views/RunwayApp.vue'),
    },
    {
      path: '/dashboard',
      component: () => import('./views/dashboard/DashboardPage.vue'),
    },
  ],
})

export default router
