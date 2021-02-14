<?php
require_once("connection.php");
require_once("Author.php");
require_once("Book.php");

function addBook($book)
{
    $connection = getConnection();
    $stmt = $connection->prepare("insert into Book (title, isRead, grade) values (:title, :isRead, :grade)");
    $stmt2 = $connection->prepare("insert into BookAuthor (bookId, authorId) values (:bookId, :authorId)");

    $stmt->bindValue(":title", $book->title);
    $stmt->bindValue(":grade", $book->grade);
    $stmt->bindValue(":isRead", $book->isRead);

    $stmt->execute();

    $bookId = $connection->lastInsertId();

    foreach ($book->authors as $author) {
        $stmt2->bindValue(":bookId", $bookId);
        $stmt2->bindValue(":authorId", $author->id);

        $stmt2->execute();
    }
}

function findBookById($idToFind)
{
    $books = getBooksWithAuthors();

    foreach ($books as $book) {
        if ($book->id === $idToFind) {
            return $book;
        }
    }
    return null;
}

function deleteBookById($id)
{
    $connection = getConnection();
    $stmt1 = $connection->prepare("delete from BookAuthor where bookId = :id");
    $stmt1->bindValue(":id", $id);
    $stmt1->execute();

    $stmt2 = $connection->prepare("delete from Book where Book.id = :id");
    $stmt2->bindValue(":id", $id);
    $stmt2->execute();
}

function editBook($book)
{
    $connection = getConnection();
    $stmt = $connection->prepare("update Book
                                            set title = :title,
                                            isRead = :isRead,
                                            grade = :grade 
                                            where Book.id = :id");

    $stmt->bindParam(":id", $book->id);
    $stmt->bindParam(":title", $book->title);
    $stmt->bindParam(":isRead", $book->isRead);
    $stmt->bindParam(":grade", $book->grade);

    $stmt->execute();

    $stmt2 = $connection->prepare(
        "delete from BookAuthor
            where :bookId = BookAuthor.bookId");
    $stmt2->bindParam(":bookId", $book->id);
    $stmt2->execute();

//           and BookAuthor.authorId != :authorId");

    $stmt3 = $connection->prepare("insert into BookAuthor (bookId, authorId) values (:bookId, :authorId)");
    foreach ($book->authors as $author) {

        $stmt3->bindParam(":bookId", $book->id);
        $stmt3->bindParam(":authorId", $author->id);
        $stmt3->execute();

    }
}

function getBooksWithAuthors()
{
    $connection = getConnection();
    $stmt = $connection->prepare("select Book.Id as bookId, title, Book.grade as bookGrade, Book.isRead,
                                    Author.Id as authorId, firstName, lastName, Author.grade as authorGrade from Book
                                    left join BookAuthor on Book.id = BookAuthor.bookId
                                    left join Author on Author.id = BookAuthor.authorId");
    $stmt->execute();

    $books = [];
    $added = false;
    foreach ($stmt as $each) {
        foreach ($books as $book) {
            if ($book->id === $each["bookId"]) {
                $book->addAuthor(new Author($each["authorId"], $each["firstName"], $each["lastName"], $each["authorGrade"]));
                $added = true;
                break;
            }
        }
        if (!$added) {
            $newBook = new Book($each["bookId"], $each["title"], $each["isRead"], $each["bookGrade"]);
            $newBook->addAuthor(new Author($each["authorId"], $each["firstName"], $each["lastName"], $each["authorGrade"]));
            $books[] = $newBook;
        }
        $added = false;
    }
    return $books;
}