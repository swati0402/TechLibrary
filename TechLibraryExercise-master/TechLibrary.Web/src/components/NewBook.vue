<template>
    <div>
        <div>
            <h2>Add New Book</h2>
            <br />
            <table style="width:100%">
                <tr>
                    <td style="width:15%"><label><b>Title:</b></label></td>
                    <td><input v-model="book.title" placeholder="Add Title" style="width:100%" required v-on:change="titleChange"></td>
                </tr>
                <tr><td><label><b>ISBN:</b></label></td><td><input v-model="book.isbn" placeholder="Add ISBN" style="width:100%"></td></tr>
                <tr><td><label><b>PublishedDate:</b></label></td><td style="width:100%"><datepicker v-model="book.publishedDate" placeholder="Select Publish Date"></datepicker></td></tr>
                <tr><td><label><b>ThumbnailUrl:</b></label></td><td><input v-model="book.thumbnailUrl" placeholder="Add Image path" style="width:100%"></td></tr>
                <tr><td><label><b>ShortDescription:</b></label></td>
                    <td>
                        <b-form-textarea v-model="book.descr" placeholder="Add Book Description (max 1000 characters)" style="width:90%" rows="6" :maxlength="max">
                        </b-form-textarea>
                        <div style="font-weight:bold; font-size:x-small" v-text="book.descr  ? (max - book.descr.length) : max"></div>
                    </td>
                </tr>
            </table>
            <br />
            <table width="70%">
                <tr>
                    <td align="center"><b-button v-on:click="addBook" variant="success" :disabled="isActive">Add Book</b-button></td>
                    <td align="left"><b-button v-on:click="reset" :disabled="isActive">Reset</b-button></td>
                </tr>
            </table>
            <br />
            <br />
            <table style="width:100%">
                <tr>
                    <td align="left"><b-button to="/" variant="primary">Back</b-button></td>
                    <td><div class="error">{{error}}</div></td>
                </tr>
            </table>
            <div class="saveMessage">{{ saveStatus }}</div>
        </div>
        </div>
    </template>
    <script>
        import axios from 'axios';
        import Datepicker from 'vuejs-datepicker';
        export default {
            components: { Datepicker },
            name: 'NewBook',
            data: () => ({
                booktitle: "",
                book: {
                    title: "",
                    isbn: "",
                    publishedDate: "",
                    thumbnailUrl: "",
                    descr: ""
                },
                error: null,
                isActive: false,
                saveStatus: "",
                max: 1000,
                redirectUrl:""
            }),
            methods: {
                //call api to add book
                addBook() {
                    //validate inputs
                    if (this.book.title == null || this.book.title.trim() == "") {
                        this.error = "* Title is required!";
                    }
                    //all valid than call add method
                    else {
                        this.error = null;
                        const reqData = JSON.stringify(this.book);
                        console.log(reqData);

                        const config = {
                            headers: {
                                "Content-Type": "application/json",
                                Accept: "application/json",
                            },
                        };
                        axios
                            .post(`https://localhost:5001/books`, reqData, config)
                            .then(
                                (response) => (
                                    (this.book = response.data),
                                    this.isActive = true,
                                    this.saveStatus = "Book added successfully.",
                                    this.redirectUrl = 'book/' +  this.book.bookId ,
                                    this.$router.push(this.redirectUrl)
                                )
                            )
                    }
                },

                //rest add book page
                reset() {
                    this.book.title = "";
                    this.book.isbn = "";
                    this.book.publishedDate = null;
                    this.book.thumbnailUrl = "";
                    this.book.descr = "";
                },

                //reset error message
                titleChange() {
                    if (this.book.title != null || this.book.title.trim() != "") {
                        this.error = "";
                    }
                },

                //check max char lenthg textarea
                assertMaxChars: function () {
                    if (this.book.descr.length >= this.maxLengthInCars) {
                        this.book.descr = this.book.descr.substring(0, this.maxLengthInCars);
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
