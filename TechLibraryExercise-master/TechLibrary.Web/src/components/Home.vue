<template>
    <div class="home">
        <h1>{{ msg }}</h1>
        <table><tr>
    <td>
        <div>
            <v-pagination v-model="currentPage"
                          :page-count="totalPages"
                          :classes="bootstrapPaginationClasses"
                          :labels="paginationAnchorTexts"></v-pagination>
        </div>
    </td>
    <td style="padding-left: 100px;">
        <div>
            <b-button to="/new" variant="primary">Add New Book</b-button>
        </div>
    </td>
</tr></table>
        <table><tr><td><div class="search-bar">
                <b-form-input @input="search_text()"
                              type="search"
                              v-model="search.text"
                              style="width:400px;"
                              placeholder="Search by Title/Description">
                </b-form-input>
                <span class="search-icon">
                    <i class="fas fa-search"></i>
                </span>
            </div>
            </td>
            </tr>
        </table>
        <span>
            
        </span>
        <b-table striped hover 
                 :items="dataContext" 
                 :fields="fields" responsive="sm"
                 :per-page="pageSize"
                 :current-page="currentPage">
            <template v-slot:cell(thumbnailUrl)="data">
                <b-img :src="data.value" thumbnail fluid></b-img>
            </template>
            <template v-slot:cell(title_link)="data">
                <b-link :to="{ name: 'book_view', params: { 'id' : data.item.bookId } }">{{ data.item.title }}</b-link>
            </template>
        </b-table>
    </div>
</template>
<script>
    import axios from 'axios';
    import vPagination from 'vue-plain-pagination'

    export default {
        name: 'Home',
        components: { vPagination },
        props: {
            msg: String
        },
        data: () => ({
            fields: [
                { key: 'thumbnailUrl', label: 'Book Image' },
                { key: 'title_link', label: 'Book Title', sortable: true },
                { key: 'isbn', label: 'ISBN', sortable: true },
                { key: 'descr', label: 'Description', sortable: true }

            ],
            items: [],
            baseapiurl: "https://localhost:5001/books",
            search: { filter: null, text: "" },
            currentPage: 1,
            totalPages: 40,
            pageSize: 10,
            bootstrapPaginationClasses: {
                ul: 'pagination',
                li: 'page-item',
                liActive: 'active',
                liDisable: 'disabled',
                button: 'page-link'
            },
            paginationAnchorTexts: {
                first: 'First',
                prev: 'Previous',
                next: 'Next',
                last: 'Last'
            }
        }),
        methods: {
            dataContext(ctx, callback) {
                axios.get(this.baseapiurl + "?pageNumber=" + this.currentPage)
                    .then(response => {                     
                        callback(response.data);
                    });
            },
            search_text() {
                //console.log(this.search.text);
                if (this.search.text == "") {
                    window.location.reload();
                }
                else {
                    axios.get(this.baseapiurl + "?searchQuery=" + this.search.text)
                        .then(response => {
                            this.dataContext = response.data;
                        });
                }
            },
        },
        beforeMount() {
            axios.get(this.baseapiurl + "/total")
                .then(response => {
                    this.totalPages = Math.ceil(response.data / this.pageSize);
                });
        }
    };
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
</style>

