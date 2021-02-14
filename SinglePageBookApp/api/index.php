<?php

require_once("dataAccess/Book.php");
require_once("dataAccess/BookDaoClass.php");
require_once("dataAccess/authorDao.php");
require_once("validations/bookValidations.php");
require_once("dataAccess/functions.php");

$bookDao = new BookDaoClass();

$cmd = "book-list";
if (isset($_GET["cmd"])) {
    $cmd = $_GET["cmd"];
}

if ($cmd === "book-list") {

    $books = $bookDao->getBooksWithAuthors();
    printJson($books);

} else if ($cmd === "book-edit") {

    $id = intval($_GET["id"]);
    $book = $bookDao->findBookById($id);

    if ($book == null) {
        http_response_code(404);
    } else {
        printJson($book);
    }

} else if ($cmd === "book-add") {

    $json = file_get_contents("php://input");
    $bookData = json_decode($json, true);

    $book = Book::fromAssocArray($bookData);
    $errors = validateBook($book);

    $author1 = findAuthorById($bookData["author1"]);
    $author2 = findAuthorById($bookData["author2"]);

    if (count($errors) > 0) {
        http_response_code(400);
        printJson(["errors" => $errors]);
    } else {
        if ($author1 !== null) {
            $book->addAuthor($author1);
        }
        if ($author2 !== null) {
            $book->addAuthor($author2);
        }
        $bookDao->addBook($book);
        printJson($book);
    }

} else if ($cmd === "edit") {

    $json = file_get_contents("php://input");
    $bookData = json_decode($json, true);
    $id = $_GET["id"];

    $book = BOok::fromAssocArray($bookData);
    $book->id = $id;
    $errors = validateBook($book);

    if (count($errors) > 0) {
        http_response_code(400);
        printJson(["errors" => $errors]);
    } else {
        $bookDao->editBook($book);
        printJson($book);
    }

} else if ($cmd === "delete") {

    $id = intval($_GET["id"]);
    $bookDao->deleteBookById($id);
    http_response_code(204);

} else if ($cmd === "getAuthors") {
    $authors = getAuthors();
    printJson($authors);
} else {
    http_response_code(400);
    printJson(["error" => "Unknown command: ${cmd}"]);
}

