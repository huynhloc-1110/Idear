// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function crudIndex(modelIdName, controllerName) {
    let btnShowModel = $('#btn-show-modal');
    let btnCreate = $('#btn-create');
    let btnEdit = $('#btn-edit');
    let btnDelete = $('#btn-delete');
    let backdrop = $('#backdrop');
    let form = $('#form-modal');

    // show modal button
    btnShowModel.click(function () {
        clearForm();
        disablePropEdit(false);
        showModalWith(btnCreate);
    })

    // create item
    btnCreate.click(function (e) {
        // check if form is valid
        if (form.valid()) {
            $.ajax({
                url: `/Admin/${controllerName}/Create`,
                type: "POST",
                data: form.serialize(),
                success: function (result) {
                    location.reload();
                }
            });
        }
    });

    // show modal to edit the targetted item
    $('button[data-modal-action=Edit]').click(function () {
        var itemId = this.getAttribute('data-modal-id');
        $.ajax({
            url: `/Admin/${controllerName}/Details/${itemId}`,
            type: "GET",
            data: form.serialize(),
            success: function (result) {
                updateForm(result);
            }
        });

        disablePropEdit(false);
        showModalWith(btnEdit);
    });

    // edit item
    btnEdit.click(function (e) {
        var itemId = $(`#${modelIdName}`).val();

        // check if form is valid
        if (form.valid()) {
            $.ajax({
                url: `/Admin/${controllerName}/Edit/${itemId}`,
                type: "PUT",
                data: form.serialize(),
                success: function (result) {
                    location.reload();
                }
            });
        }
    })

    // show modal to delete the targetted item
    $('button[data-modal-action=Delete]').click(function () {
        var itemId = this.getAttribute('data-modal-id');
        $.ajax({
            url: `/Admin/${controllerName}/Details/${itemId}`,
            type: "GET",
            data: $('#form-modal').serialize(),
            success: function (result) {
                updateForm(result);
            }
        });

        disablePropEdit(true);
        showModalWith(btnDelete);
    });

    // delete item
    btnDelete.click(function (e) {
        var itemId = $(`#${modelIdName}`).val();

        // check if form is valid
        if (form.valid()) {
            $.ajax({
                url: `/Admin/${controllerName}/Delete/${itemId}`,
                type: "DELETE",
                data: form.serialize(),
                success: function (result) {
                    location.reload();
                }
            });
        }
    })

    // hide unncessary buttons and popup the modal
    function showModalWith(shownButton) {
        btnCreate.hide();
        btnEdit.hide();
        btnDelete.hide();

        shownButton.show();
        backdrop.modal('show');
    }
}

// highlight fragment
$(function() {
    let fragment = window.location.hash;
    if (fragment) {
        $(fragment).addClass("highlight");
        $(fragment).click(function () {
            $(fragment).removeClass("highlight");
        });
    }
})
