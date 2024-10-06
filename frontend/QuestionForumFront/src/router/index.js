import { createRouter, createWebHistory } from 'vue-router'
import Home from '../views/Home.vue'
import QuestionDetail from '../views/QuestionDetail.vue'
import Login from '../views/Login.vue';
import SignUp from '../views/SignUp.vue'
import NotFound from '../views/NotFound.vue';  // 404 sayfanız

const routes = [
  {
    path: '/',
    name: 'Home',
    component: Home
  },
  
  {
    path: '/question/:id', 
    name: 'QuestionDetail',
    component: QuestionDetail
  },

  {
    path: '/login',
    name: 'Login',
    component: Login
  },

  {
    path: '/signup',
    name: 'SignUp',
    component: SignUp
  },

  {
    path: '/:catchAll(.*)',
    name: 'NotFound',
    component: NotFound
  }
]

const router = createRouter({
  history: createWebHistory(process.env.BASE_URL),
  routes
})

export default router