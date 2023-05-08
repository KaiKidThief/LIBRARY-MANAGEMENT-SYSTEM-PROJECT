﻿using LibraryManagementSystemWF.dao;
using LibraryManagementSystemWF.models;
using LibraryManagementSystemWF.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LibraryManagementSystemWF.controllers
{
    internal class BookController : BaseController
    {

        public static ControllerModifyData<Book> CreateBook(
            int? genreId,
            string title,
            string author,
            string publisher,
            DateTime publicationDate,
            string isbn,
            string coverPath = "",
            string sypnosis = "No sypnosis available"
            )
        {
            ControllerModifyData<Book> returnData = new ControllerModifyData<Book>();
            Dictionary<string, string> errors = new Dictionary<string, string>();
            bool isSuccess = false;

            // is not admin
            if (!AuthGuard.IsAdmin())
            {
                errors.Add("permission", "Forbidden");
                returnData.Errors = errors;
                returnData.IsSuccess = false;

                return returnData;
            }

            // validation
            if (!Validator.IsNameUnique("books", "title", title)) errors.Add("title", "Title already exists");
            if (!Validator.IsGenreIdValid(genreId)) errors.Add("genreId", "ID is invalid");
            if (string.IsNullOrWhiteSpace(title)) errors.Add("title", "Title is required");
            if (string.IsNullOrWhiteSpace(author)) errors.Add("author", "Author is required");
            if (string.IsNullOrWhiteSpace(publisher)) errors.Add("publisher", "Publisher is required");
            if (!Validator.IsDateBeforeOrOnPresent(publicationDate)) errors.Add("publicationDate", "Datetime must be before or on the present date");
            if (!Validator.IsValidISBN(isbn)) errors.Add("isbn", "Invalid ISBN. Make sure the ISBN is in ISBN-10 or ISBN-13 format");

            if (errors.Count == 0)
            {
                BookDAO bookDao = new BookDAO();
                ReturnResult<Book> result = bookDao.Create(new Book
                {
                    Title = title,
                    Sypnosis = sypnosis,
                    Author = author,
                    Cover = coverPath,
                    Publisher = publisher,
                    PublicationDate = publicationDate,
                    ISBN = isbn,
                    Genre = new Genre
                    {
                        ID = genreId
                    }
                });

                isSuccess = result.IsSuccess;
                returnData.Result = result.Result;
            }

            returnData.Errors = errors;
            returnData.IsSuccess = isSuccess;
            return returnData;
        }

        public static ControllerModifyData<Book> UpdateBook(
            string bookId,
            int genreId,
            string title,
            string author,
            string publisher,
            DateTime publicationDate,
            string isbn,
            string coverPath = "",
            string sypnosis = "No sypnosis available"
            )
        {
            ControllerModifyData<Book> returnData = new ControllerModifyData<Book>();
            Dictionary<string, string> errors = new Dictionary<string, string>();
            bool isSuccess = false;

            // is not admin
            if (!AuthGuard.IsAdmin())
            {
                errors.Add("permission", "Forbidden");
                returnData.Errors = errors;
                returnData.IsSuccess = false;

                return returnData;
            }

            // validation
            if (!Validator.IsNameUnique("books", "title", title)) errors.Add("title", "Title already exists");
            if (!Validator.IsGenreIdValid(genreId)) errors.Add("genreId", "ID is invalid");
            if (string.IsNullOrWhiteSpace(title)) errors.Add("title", "Title is required");
            if (string.IsNullOrWhiteSpace(author)) errors.Add("author", "Author is required");
            if (string.IsNullOrWhiteSpace(publisher)) errors.Add("publisher", "Publisher is required");
            if (!Validator.IsDateBeforeOrOnPresent(publicationDate)) errors.Add("publicationDate", "Datetime must be before or on the present date");
            if (!Validator.IsValidISBN(isbn)) errors.Add("isbn", "Invalid ISBN. Make sure the ISBN is in ISBN-10 or ISBN-13 format");

            // update if theres no error
            if (errors.Count == 0)
            {
                BookDAO bookDao = new BookDAO();

                // check if book exists
                ReturnResult<Book> book = bookDao.GetById(bookId);

                if (!book.IsSuccess)
                {
                    errors.Add("bookId", "Book not found");
                    returnData.Errors = errors;
                    returnData.IsSuccess = isSuccess;
                    return returnData;
                }

                // proceed if book is found
                ReturnResult<Book> result = bookDao.Update(new Book
                {
                    ID = new Guid(bookId),
                    Title = title,
                    Sypnosis = sypnosis,
                    Author = author,
                    Cover = coverPath,
                    Publisher = publisher,
                    PublicationDate = publicationDate,
                    ISBN = isbn,
                    Genre = new Genre
                    {
                        ID = genreId
                    }
                });

                Console.WriteLine("RESULT: " + result.IsSuccess);
                isSuccess = result.IsSuccess;
                if (isSuccess && result.Result != null)
                {
                    returnData.Result = result.Result;
                }
            }

            returnData.Errors = errors;
            returnData.IsSuccess = isSuccess;
            return returnData;
        }

        public static ControllerModifyData<Book> GetBookById(string id)
        {
            ControllerModifyData<Book> returnData = new ControllerModifyData<Book>();
            returnData.Result = default(Book);
            Dictionary<string, string> errors = new Dictionary<string, string>();
            bool isSuccess = false;

            // is not admin
            if (!AuthGuard.IsAdmin())
            {
                errors.Add("permission", "Forbidden");
                returnData.Errors = errors;
                returnData.IsSuccess = false;

                return returnData;
            }

            // validate fields
            if (string.IsNullOrWhiteSpace(id)) errors.Add("id", "ID is invalid");

            if (errors.Count == 0)
            {
                BookDAO bookDao = new BookDAO();
                ReturnResult<Book> result = bookDao.GetById(id);

                isSuccess = result.IsSuccess;
                if (isSuccess && result.Result != null)
                {
                    returnData.Result = result.Result;
                }
            }

            returnData.Errors = errors;
            returnData.IsSuccess = isSuccess;
            return returnData;
        }

        public static ControllerAccessData<Book> GetAllBooks(int page = 1)
        {
            ControllerAccessData<Book> returnData = new ControllerAccessData<Book>();
            returnData.Results = new List<Book>();
            Dictionary<string, string> errors = new Dictionary<string, string>();
            bool isSuccess = false;
            returnData.rowCount = 0;

            // is not admin
            if (!AuthGuard.IsAdmin())
            {
                errors.Add("permission", "Forbidden");
                returnData.Errors = errors;
                returnData.IsSuccess = false;

                return returnData;
            }

            if (page <= 0) errors.Add("page", "Invalid page");

            if (errors.Count == 0)
            {
                BookDAO bookDao = new BookDAO();
                ReturnResultArr<Book> result = bookDao.GetAll(page);

                isSuccess = result.IsSuccess;
                returnData.Results = result.Results;
                returnData.rowCount = result.rowCount;
            }

            returnData.Errors = errors;
            returnData.IsSuccess = isSuccess;
            return returnData;
        }

        public static ControllerActionData RemoveById(string id)
        {
            ControllerActionData returnResult = new ControllerActionData();
            returnResult.Errors = new Dictionary<string, string>();
            returnResult.IsSuccess = false;

            // is not admin
            if (!AuthGuard.IsAdmin())
            {
                returnResult.Errors.Add("permission", "Forbidden");

                return returnResult;
            }

            if (returnResult.Errors.Count == 0)
            {
                BookDAO bookDao = new BookDAO();
                returnResult.IsSuccess = bookDao.Remove(id);
            }

            return returnResult;
        }
    }
}
