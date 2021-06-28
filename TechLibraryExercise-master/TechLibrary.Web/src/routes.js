import Vue from 'vue';
import VueRouter from 'vue-router';

Vue.use(VueRouter);

const Home = () => import(/* webpackChunkName: "Home" */ './components/Home.vue');
const Book = () => import(/* webpackChunkName: "Book" */ './components/Book.vue');
const NewBook = () => import(/* webpackChunkName: "NewBook" */ './components/NewBook.vue');

const router = new VueRouter({
  routes: [
        { path: '/', component: Home },
        { path: '/new', component: NewBook },
    {
      name: 'book_view',
      path: '/book/:id',
      component: Book,
      props: true
    }
  ]
});

export default router;