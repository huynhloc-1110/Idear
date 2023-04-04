let btnEdit = $('#btn-edit');
let btnDelete = $('#btn-delete');
let backdrop = $('#backdrop');
let form = $('#form-modal');

// show modal to edit the targetted item
$('button[data-modal-action=Edit]').click(function () {
    var itemId = this.getAttribute('data-modal-id');
    $.ajax({
        url: `/Staff/Comments/Details/${itemId}`,
        type: "GET",
        success: function (result) {
            updateForm(result);
        }
    });

    disablePropEdit(false);
    showModalWith(btnEdit);
});

// edit item
btnEdit.click(function (e) {
    var itemId = $('#Id').val();

    // check if form is valid
    if (form.valid()) {
        $.ajax({
            url: `/Staff/Comments/Edit/${itemId}`,
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
        url: `/Staff/Comments/Details/${itemId}`,
        type: "GET",
        success: function (result) {
            updateForm(result);
        }
    });

    disablePropEdit(true);
    showModalWith(btnDelete);
});

// delete item
btnDelete.click(function (e) {
    var itemId = $('#Id').val();

    // check if form is valid
    if (form.valid()) {
        $.ajax({
            url: `/Staff/Comments/Delete/${itemId}`,
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
    btnEdit.hide();
    btnDelete.hide();

    shownButton.show();
    backdrop.modal('show');
}

// used for disabling form editting when deleting a category
function disablePropEdit(disable) {
    $('#Text').prop('disabled', disable);
    $('#IsAnonymous').prop('disabled', disable);
}

// used for update the form with json response from Details action
function updateForm(result) {
    $('#Id').val(result.id);
    $('#Text').val(result.text);
    $('#IsAnonymous').prop('checked', result.isAnonymous);
}
