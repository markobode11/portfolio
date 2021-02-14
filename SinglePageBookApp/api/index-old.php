<?php

require_once 'vendor/tpl.php';
require_once("dataAccess/bookDao.php");
require_once("dataAccess/authorDao.php");
require_once("dataAccess/Author.php");
require_once("dataAccess/Book.php");
require_once("validations/authorValidations.php");
require_once("validations/bookValidations.php");


$cmd = isset($_REQUEST['cmd'])
    ? $_REQUEST['cmd']
    : 'book-list';


if ($cmd == 'book-list') {
    $data = [
        'template' => 'book-list.html',
        'books' => getBooksWithAuthors(),
        'message' => isset($_GET['msg']) ? $_GET['msg'] : ''
    ];


    print renderTemplate('tpl/main.html', $data);


} else if ($cmd == 'author-list') {
    $data = [
        'template' => 'author-list.html',
        'authors' => getAuthors(),
        'message' => isset($_GET['msg']) ? $_GET['msg'] : ''
    ];

    print renderTemplate('tpl/main.html', $data);

} else if ($cmd == 'book-add') {
    if ($_SERVER["REQUEST_METHOD"] === "GET") {
        $data = [
            'template' => 'book-add.html',
            'book' => isset($_GET["book"]) ? unserialize(urldecode($_GET["book"])) : new Book("", "", "", 0),
            'editForm' => false,
            'authors' => getAuthors(),
            'errors' => isset($_GET["errors"]) ? unserialize(urldecode($_GET["errors"])) : []
        ];
        print renderTemplate('tpl/main.html', $data);
    } else {
        $isRead = isset($_POST["isRead"]) ? $_POST["isRead"] : "";
        $grade = isset($_POST["grade"]) ? $_POST["grade"] : 0;
        $addedBook = new Book("", $_POST["title"], $isRead, $grade);
        if ($_POST["author1"]) {
            $author1 = findAuthorById($_POST["author1"]);
            $addedBook->addAuthor($author1);
        }
        if ($_POST["author2"]) {
            $author2 = findAuthorById($_POST["author2"]);
            $addedBook->addAuthor($author2);
        }

        $errors = validateBook($addedBook);

        if (sizeof($errors) > 0) {
            $errors = urlencode(serialize($errors));
            $addedBook = urlencode(serialize($addedBook));
            header("Location: index-old.php?cmd=book-add&errors=$errors&book=$addedBook");
        } else {
            addBook($addedBook);
            header("Location: index-old.php?cmd=book-list&msg=Lisatud!");
        }
    }
} else if ($cmd == 'author-add') {
    if ($_SERVER["REQUEST_METHOD"] === "GET") {
        $data = [
            'template' => 'author-add.html',
            'author' => isset($_GET["author"]) ? unserialize(urldecode($_GET["author"])) : new Author("", "", "", ""),
            'editForm' => false,
            'errors' => isset($_GET["errors"]) ? unserialize(urldecode($_GET["errors"])) : []
        ];
        print renderTemplate('tpl/main.html', $data);
    } else {
        $grade = isset($_POST["grade"]) ? $_POST["grade"] : 0;
        $addedAuthor = new Author('', $_POST["firstName"], $_POST["lastName"], $grade);

        $errors = validateAuthor($addedAuthor);

        if (sizeof($errors) > 0) {
            $errors = urlencode(serialize($errors));
            $addedAuthor = urlencode(serialize($addedAuthor));
            header("Location: index-old.php?cmd=author-add&errors=$errors&author=$addedAuthor");
        } else {
            addAuthor($addedAuthor);
            header("Location: index-old.php?cmd=author-list&msg=Lisatud!");
        }
    }
} else if ($cmd == 'edit-book') {
    if ($_SERVER["REQUEST_METHOD"] === "GET") {
        $currentBook = findBookById($_GET["id"]);
        $data = [
            'template' => 'edit-book.html',
            'book' => isset($_GET["book"]) ? unserialize(urldecode($_GET["book"])) : $currentBook,
            'editForm' => true,
            'authors' => getAuthors(),
            'errors' => isset($_GET["errors"]) ? unserialize(urldecode($_GET["errors"])) : []
        ];
        print renderTemplate('tpl/main.html', $data);
    } else {
        if (isset($_POST["deleteButton"])) {
            $id = $_POST["id"];
            deleteBookById($id);
            header("Location: index-old.php?cmd=book-list&msg=Kustutatud!");
        } else {
            $currentBook = findBookById($_POST["id"]);
            $currentBook->title = $_POST["title"];
            $currentBook->isRead = isset($_POST["isRead"]) ? $_POST["isRead"] : "";
            $currentBook->grade = isset($_POST["grade"]) ? $_POST["grade"] : 0;
            $currentBook->authors = [];

            if ($_REQUEST["author1"]) {
                $author1 = findAuthorById($_POST["author1"]);
                $currentBook->addAuthor($author1);
            }
            if ($_REQUEST["author2"]) {
                $author2 = findAuthorById($_POST["author2"]);
                $currentBook->addAuthor($author2);
            }

            $errors = validateBook($currentBook);

            if (sizeof($errors) > 0) {
                $errors = urlencode(serialize($errors));
                $currentBook = urlencode(serialize($currentBook));
                header("Location: index-old.php?cmd=edit-book&errors=$errors&book=$currentBook");
            } else {
                editBook($currentBook);
                header("Location: index-old.php?cmd=book-list&msg=Muudetud!");
            }
        }
    }

} else if ($cmd == 'edit-author') {
    if ($_SERVER["REQUEST_METHOD"] === "GET") {
        $currentAuthor = findAuthorById($_GET["id"]);
        $data = [
            'template' => 'edit-author.html',
            'author' => isset($_GET["author"]) ? unserialize(urldecode($_GET["author"])) : $currentAuthor,
            'editForm' => true,
            'authors' => getAuthors(),
            'errors' => isset($_GET["errors"]) ? unserialize(urldecode($_GET["errors"])) : []
        ];
        print renderTemplate('tpl/main.html', $data);
    } else {
        if (isset($_POST["deleteButton"])) {
            $id = $_POST["id"];
            deleteAuthorById($id);
            header("Location: index-old.php?cmd=author-list&msg=Kustutatud!");
        } else {
            $currentAuthor = findAuthorById($_POST["id"]);
            $currentAuthor->firstName = $_POST["firstName"];
            $currentAuthor->lastName = $_POST["lastName"];
            $currentAuthor->grade = isset($_POST["grade"]) ? $_POST["grade"] : 0;

            $errors = validateAuthor($currentAuthor);

            if (sizeof($errors) > 0) {
                $errors = urlencode(serialize($errors));
                $currentAuthor = urlencode(serialize($currentAuthor));
                header("Location: index-old.php?cmd=edit-author&errors=$errors&book=$currentAuthor");
            } else {
                editAuthor($currentAuthor);
                header("Location: index-old.php?cmd=author-list&msg=Muudetud!");
            }
        }
    }
} else {
    throw new RuntimeException('No such command defined!');
}
