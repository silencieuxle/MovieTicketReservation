function index_loadMoviesFilterData(type) {
	$.getJSON("/Home/GetMoviesByScheduleType", { type: type }, function (data) {
		$.each(data, function (index, value) {
			var htmlString = "<div class='col-xs-6 col-md-3 film-thumbnail'><a href='/Movies/Details?movieID=" + value.MovieId + "' class='thumbnail'><img src='" + value.ThumbnailUrl + "' /></a></div>";
			$(".filtered-movies").append(htmlString);
		});
	});
}

function index_loadCinemaData(movieId) {
	$('#selCinema').empty();
	$('#selCinema').append('<option value="" disabled selected>Chọn rạp</option>');
	$.getJSON('/Home/GetCinemasByMovieID', { movieID: movieId }, function (data) {
		$.each(data, function (index, value) {
			var htmlString = "<option value='" + value.ID + "'>" + value.Name + "</option>";
			$("#selCinema").append(htmlString);
		});
	});
}

function index_loadScheduleDateData(movieId, cinemaId) {
	$("#selDate").empty();
	$("#selDate").append("<option value=\"\" disabled selected>Chọn ngày</option>");
	$.getJSON("/Home/GetDateByCinemaIDAndMovieID", { movieID: movieId, cinemaID: cinemaId }, function (data) {
		$.each(data, function (index, value) {
			var date = new Date(parseInt(value.Date.substr(6)));
			var htmlString = "<option value='" + value.ScheduleID + "'>" + date.toLocaleDateString("vi-VI") + "</option>";
			$("#selDate").append(htmlString);
		});
	});
}

function index_loadScheduleShowTimeData(scheduleId, movieId) {
	$("#selShowtime").empty();
	$("#selShowtime").append("<option value=\"\" disabled selected>Chọn suất</option>");
	$.getJSON("/Home/GetShowtimeByScheduleID", { scheduleID: scheduleId, movieID: movieId }, function (data) {
		$.each(data, function (index, value) {
			var htmlString = "<option value='" + value.ScheduleID + "'>" + value.Time.Hours + " giờ " + value.Time.Minutes + " phút" + "</option>";
			$("#selShowtime").append(htmlString);
		});
	});
}

function reserveTicket(scheduleId) {
	window.location.href = "/Ticket/Reserve?scheduleId=" + scheduleId;
}