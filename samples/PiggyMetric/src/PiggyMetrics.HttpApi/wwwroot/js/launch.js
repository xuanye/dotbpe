var global = {
    mobileClient: false,
    savePermit: true,
    usd: 0,
    eur: 0,
    login:false
};

function doLogout(){

	$.ajax({
		url: 'auth/logout',
		datatype: 'json',
		type: 'get',
		async: false,
		success: function (data) {
		},
		error: function () {

		}
	});
}

function doLogin(username, password) {

	var success = false;

	$.ajax({
		url: 'auth/login',
		datatype: 'json',
        type: 'post',
        contentType: "application/json",
        async: false,
        data: JSON.stringify({
            account: username,
			password: password,

		}),
		success: function (data) {
            if(data.status ==0){
			    success = true;
                global.login = true;
            }

		},
		error: function () {

		}
	});

	return success;
}


/**
 * Current account
 */

function getCurrentAccount() {

	var account = null;

	if (true) {
		$.ajax({
			url: 'accounts/',
			datatype: 'json',
			type: 'get',
			async: false,
			success: function (ret) {
                if (ret.status == 0 ){
                    account = ret.data;
                }
			},
			error: function () {
			}
		});
	}

	return account;
}

$(window).load(function(){

	if(/Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent) ) {
		FastClick.attach(document.body);
        global.mobileClient = true;
	}

    $.getJSON("http://api.fixer.io/latest?base=RUB", function( data ) {
        global.eur = 1 / data.rates.EUR;
        global.usd = 1 / data.rates.USD;
    });

	var account = getCurrentAccount();

	if (account) {
		showGreetingPage(account);
	} else {
		showLoginForm();
	}
});

function showGreetingPage(account) {
    initAccount(account);
	var userAvatar = $("<img />").attr("src","images/userpic.jpg");
	$(userAvatar).load(function() {
		setTimeout(initGreetingPage, 500);
	});
}

function showLoginForm() {
	$("#loginpage").show();
	$("#frontloginform").focus();
	setTimeout(initialShaking, 700);
}
