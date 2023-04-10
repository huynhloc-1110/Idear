function ajaxPostReact(flag) {
	// Get the CSRF token from the server
	$.get("/Staff/Helpers/GetCsrfToken", function (token) {
		// Add the CSRF token to the AJAX request header
		var headers = {};
		headers["RequestVerificationToken"] = token;

		// Get the current idea's id
		var id = $('#idea-id').text();

		// Send the AJAX request with the CSRF token included in the header
		$.ajax({
			url: '/Staff/Reacts/Like',
			type: "POST",
			data: { reactFlag: flag, ideaId: id },
			headers: headers,
			success: function (result) {
				// update like, dislike UI after successfully change the backend
				updateReactButtons(result.flag, result.likeCount, result.dislikeCount);
			},
		});
	});
}

function updateReactButtons(flag, likeCount, dislikeCount) {
	if (flag == 1) {
		$('#btn-like > i').removeClass('bi-hand-thumbs-up').addClass('bi-hand-thumbs-up-fill');
		$('#btn-dislike > i').removeClass('bi-hand-thumbs-down-fill').addClass('bi-hand-thumbs-down');
	} else if (flag == -1) {
		$('#btn-dislike > i').removeClass('bi-hand-thumbs-down').addClass('bi-hand-thumbs-down-fill');
		$('#btn-like > i').removeClass('bi-hand-thumbs-up-fill').addClass('bi-hand-thumbs-up');
	} else {
		$('#btn-like > i').removeClass('bi-hand-thumbs-up-fill').addClass('bi-hand-thumbs-up');
		$('#btn-dislike > i').removeClass('bi-hand-thumbs-donw-fill').addClass('bi-hand-thumbs-down');
	}

	$('#btn-like > span').text(likeCount);
	$('#btn-dislike > span').text(dislikeCount);
}

function ajaxPostCmt() {
	// Get the CSRF token from the server
	$.get("/Staff/Helpers/GetCsrfToken", function (token) {
		// Add the CSRF token to the AJAX request header
		var headers = {};
		headers["RequestVerificationToken"] = token;

		// Get the necessary info for creating a new cmt
		var id = $('#idea-id').text();
		var cmtText = $('#input-cmt').val();
		var isAnonymous = $('#chk-anonymous').prop('checked');

		// Send the AJAX request with the CSRF token included in the header
		$.ajax({
			url: '/Staff/Comments/Create',
			type: "POST",
			data: { 'cmtText': cmtText, 'isAnonymous': isAnonymous, ideaId: id },
			headers: headers,
			success: function (result) {
				// update cmt UI after successfully change the backend
				updateCmtUI(result, isAnonymous, cmtText);
			},
		});
	});
}

function updateCmtUI(result, isAnonymous, cmtText) {
	// add the new comment after the 'create comment' section
	var cmtOwner = isAnonymous ? 'Anonymous User' : result.user;
	var cmtDateTime = (new Date(result.dateTime)).toLocaleString("en-GB");
	$('#card-new-cmt').after(
		`
		<div class="card">
			<div class="card-body" id="${result.id}">
				<div class="row align-items-center">
					<p class="col-6">
						<b><text>${cmtOwner}</text></b>
					</p>
					<p class="col-6 text-right">
						<button data-modal-type="new-cmt" data-modal-action="Edit" data-modal-id="${result.id}" type="button" class="btn btn-outline-primary"><i class="bi bi-pencil-square"></i></button>
						<button data-modal-type="new-cmt" data-modal-action="Delete" data-modal-id="${result.id}" type="button" class="btn btn-outline-dark"><i class="bi bi-trash"></i></button>
					</p>
				</div>
				<p>${cmtText}</p>
				<p class="text-right">
					<small class="text-muted">
						at <time class="datetime">${cmtDateTime}</time>
					</small>
				</p>
			</div>
		</div>
		`
	);

	// add event listeners for the new created comment
	$('button[data-modal-action=Edit][data-modal-type=new-cmt]').click(function () {
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
	// show modal to delete the targetted item
	$('button[data-modal-action=Delete][data-modal-type=new-cmt]').click(function () {
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

	// update comment count of this idea page
	var cmtCount = $('#cmt-count');
	cmtCount.text(parseInt(cmtCount.text()) + 1);

	// clear comment textarea and checkbox
	$('#input-cmt').val('');
	$('#chk-anonymous').prop('checked', false);
}

$(document).ready(function () {
	$('#input-cmt').on('input', function () {
		var text = $(this).val();
		var charCount = text.length;
		$('#char-count').text(charCount);
	});

	$('#btn-like').click(function () {
		ajaxPostReact(1);
	});
	$('#btn-dislike').click(function () {
		ajaxPostReact(-1);
	});
	$('#btn-new-cmt').click(ajaxPostCmt);
})
