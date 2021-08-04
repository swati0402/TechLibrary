<template>
    <div v-if="book">
        <br />
        <table>
            <tr>
                <td style="width:40%; vertical-align:central"><span><b> Edit Book</b></span></td>
                <td></td>
                <td>
                    <toggle-button :value="false"
                                   :labels="{checked: 'On', unchecked: 'Off'}"
                                   @change="onChangeEventHandler"
                                   v-model="isEditable"
                                   :sync="true" />
                </td>
            </tr>
        </table>
        <div v-if="!isEditable">
            <b-card :title="book.title"
                    :img-src="book.thumbnailUrl"
                    img-alt="Image"
                    img-top
                    tag="article"
                    style="max-width: 30rem;"
                    class="mb-2">
                <b-card-text>
                    {{ book.descr }}
                </b-card-text>

                <b-button to="/" variant="primary">Back</b-button>
            </b-card>
        </div>
        <div v-if="isEditable">
            <b-card :img-src="book.thumbnailUrl"
                    img-alt="Image"
                    img-top
                    tag="article"
                    style="max-width: 30rem;"
                    class="mb-2">
                <b-form-group label="Title:"
                              label-for="nested-street"
                              label-cols-sm="3"
                              label-align-sm="right">
                    <b-form-textarea v-model="book.title" style="width:100%" rows="1" required v-on:change="titleChange"></b-form-textarea>
                </b-form-group>
                <b-form-group label="Description:"
                              label-for="nested-street"
                              label-cols-sm="3"
                              label-align-sm="right">
                    <b-form-textarea v-model="book.descr" style="width:100%" rows="6">
                    </b-form-textarea>
                </b-form-group>
                    <table>
                        <tr>
                            <td><b-button v-on:click="updateBook" variant="primary">Save</b-button></td>
                            <td><b-button v-on:click="makeReadOnly">Cancel</b-button></td>
                        </tr>
                    </table>
                    <div class="error">{{error}}</div>
            </b-card>
        </div>

    </div>
</template>

<script>
    import axios from 'axios';
    import Vue from 'vue';
    import ToggleButton from 'vue-js-toggle-button'

    Vue.use(ToggleButton)

    export default {
        name: 'Book',
        props: ["id"],
        data: () => ({
            book: null,
            isEditable: false,
            error: null
        }),
        mounted() {
            axios.get(`https://localhost:5001/books/${this.id}`)
                .then(response => {
                    this.book = response.data;
                });
        },
        methods: {
            onChangeEventHandler(value) {
                this.isEditable = value.value;
            },
            makeReadOnly() {
                this.isEditable = false;
            },
            //reset error message
            titleChange() {
                if (this.book.title != null || this.book.title.trim() != "") {
                    this.error = "";
                }
            },
            updateBook() {
                const reqData = JSON.stringify(this.book);
                console.log(reqData);
                //validate inputs
                if (this.book.title == null || this.book.title.trim() == "") {
                    this.error = "* Title is required!";
                }
                //all valid than call add method
                else {
                    this.error = null;
                    const config = {
                        headers: {
                            "Content-Type": "application/json",
                            Accept: "application/json",
                        },
                    };
                    axios
                        .put(`https://localhost:5001/books/${this.id}`, reqData, config)
                        .then(
                            (response) => (
                                (this.book = response.data),
                                this.isEditable = false
                            )
                        );
                }
            }
        }
    }
</script>
<style>
    .saveMessage {
        color: #184e9a;
        font-weight: bold;
    }

    .error {
        color: red;
    }
</style>