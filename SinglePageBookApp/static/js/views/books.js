import { elem } from "../elements.js";

const books = async (params) => {
    document.title = "Books";

    const books = await fetch("/api/index.php?cmd=book-list")
        .then(response => response.json());

    const page = document.createElement("div");
    const title = document.createElement("h1");
    title.textContent = "Raamatud";
    page.appendChild(title);

    const tbody = elem("tbody");

    books.forEach(book => {
        let names = "";
        book.authors.forEach(author => {
            if (author.firstName !== null && author.lastName !== null) {
                names += author.firstName + " " + author.lastName + ", "
            }
            });

        names = names.substring(0, names.length - 2);

        const row = elem("tr",
            elem("td", book.id),
            elem("td", linkToBook(book)),
            elem("td", names),
            elem("td", book.isRead),
            elem("td", book.grade)
        );

        tbody.appendChild(row);
    });

    const table = elem("table",
        elem("thead",
            elem("tr",
                elem("th", "ID"),
                elem("th", "Pealkiri"),
                elem("th", "Autorid"),
                elem("th", "Loetud"),
                elem("th", "Hinne")
            )
        ),
        tbody
    )
    table.className = "table";

    if (params.get('success') && params.get('success') === "true") {
        const alert = elem("div", "Salvestamine õnnestus");
        alert.className = "alert alert-success";
        alert.role = "alert";

        page.appendChild(alert);
    }
    if (params.get('deleted') && params.get("deleted") === "true") {
        const alert = elem("div", "Kustutamine õnnestus");
        alert.className = "alert alert-success";
        alert.role = "alert";

        page.appendChild(alert);
    }

    page.appendChild(table);
    return page;
}
//
// function deleteContactButton(contact) {
//     const button = elem("button", "Kustuta");
//     button.className = "btn btn-danger";
//     button.type = "button";
//
//     button.addEventListener("click", e => {
//         fetch(`/api/index.php?cmd=delete&id=${contact.id}`, { method: "POST" })
//             .then(response => {
//                 if (response.ok) {
//                     e.target.closest("tr").remove();
//                 }
//             });
//     });
//
//
//     return button;
// }

function linkToBook(book) {
    const link = elem("a", book.title);
    link.href = `/edit-book?id=${encodeURIComponent(book.id)}`;
    link.setAttribute("data-link", "");
    return link;
}

export default books;