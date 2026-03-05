import { createRouter, createWebHistory } from 'vue-router'

const router = createRouter({
  history: createWebHistory(),
  routes: [
    {
      path: '/',
      component: () => import('./views/dashboard/DashboardPage.vue'),
    },
    {
      path: '/runway',
      component: () => import('./views/RunwayApp.vue'),
    },
  ],
})

export default router
