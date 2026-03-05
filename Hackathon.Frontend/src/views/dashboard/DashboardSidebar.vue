<template>
  <spr-sidenav
    :nav-links="navLinks"
    :active-nav="activeNav"
    :user-menu="userMenu"
    :has-search="false"
    @get-navlink-item="handleNavClick"
  >
    <template #logo-image>
      <img src="/sprout-icon.svg" alt="Sprout" style="width:36px;height:36px;object-fit:contain;" />
    </template>
  </spr-sidenav>
</template>

<script setup lang="ts">
import { ref, computed, watch } from 'vue'
import { useRunwayV4Store } from '../../stores/runway-v4'

const store = useRunwayV4Store()

const activeNav = ref({
  parentNav: 'Dashboard',
  menu: '',
  submenu: '',
})

const userMenu = ref({
  name: store.payroll?.employeeName || 'Jane Doe',
  email: 'jane@company.com',
  profileImage: '',
  items: [
    {
      title: 'Profile',
      icon: 'ph:user',
      hidden: false,
      redirect: { openInNewTab: false, isAbsoluteURL: false, link: '#profile' },
    },
    {
      title: 'Sign Out',
      icon: 'ph:sign-out',
      hidden: false,
      redirect: { openInNewTab: false, isAbsoluteURL: false, link: '#logout' },
    },
  ],
})

const navLinks = ref({
  top: [
    {
      parentLinks: [
        {
          title: 'Dashboard',
          icon: 'ph:house-simple',
          redirect: { openInNewTab: false, isAbsoluteURL: false, link: '#dashboard' },
          menuLinks: [],
          submenuLinks: [],
          attributes: [],
        },
        {
          title: 'Analytics',
          icon: 'ph:chart-bar',
          redirect: { openInNewTab: false, isAbsoluteURL: false, link: '#analytics' },
          menuLinks: [],
          submenuLinks: [],
          attributes: [],
        },
        {
          title: 'Apps',
          icon: 'ph:squares-four',
          redirect: { openInNewTab: false, isAbsoluteURL: false, link: '#apps' },
          menuLinks: [],
          submenuLinks: [],
          attributes: [],
        },
      ],
    },
    {
      parentLinks: [
        {
          title: 'Payroll',
          icon: 'ph:money',
          redirect: { openInNewTab: false, isAbsoluteURL: false, link: '#payroll' },
          menuLinks: [],
          submenuLinks: [],
          attributes: [],
        },
        {
          title: 'Finances',
          icon: 'ph:wallet',
          redirect: { openInNewTab: false, isAbsoluteURL: false, link: '#finances' },
          menuLinks: [],
          submenuLinks: [],
          attributes: [],
        },
        {
          title: 'Flow',
          icon: 'ph:arrows-split',
          redirect: { openInNewTab: false, isAbsoluteURL: false, link: '#flow' },
          menuLinks: [],
          submenuLinks: [],
          attributes: [],
        },
      ],
    },
  ],
  bottom: [
    {
      parentLinks: [
        {
          title: 'Setup',
          icon: 'ph:gear',
          redirect: { openInNewTab: false, isAbsoluteURL: false, link: '#setup' },
          menuLinks: [],
          submenuLinks: [],
          attributes: [],
        },
        {
          title: 'Notifications',
          icon: 'ph:bell',
          redirect: { openInNewTab: false, isAbsoluteURL: false, link: '#notifications' },
          menuLinks: [],
          submenuLinks: [],
          attributes: [],
        },
        {
          title: 'Requests',
          icon: 'ph:file-text',
          redirect: { openInNewTab: false, isAbsoluteURL: false, link: '#requests' },
          menuLinks: [],
          submenuLinks: [],
          attributes: [],
        },
      ],
    },
  ],
})

watch(() => store.payroll?.employeeName, (name) => {
  if (name) userMenu.value.name = name
})

function handleNavClick(item: any) {
  if (!item) return
  activeNav.value = { parentNav: item.title, menu: '', submenu: '' }
}
</script>

<style scoped>
/* Push bottom nav icons to bottom of flex column, just above the avatar */
:deep(.spr-grid.spr-justify-center.spr-gap-2.spr-px-3.spr-pb-4.spr-pt-0) {
  margin-top: auto;
  padding-bottom: 60px;
}
</style>
