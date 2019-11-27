// JavaScript Document
function minHeight(){
	var minHeight = $(window).height();
	$('body').css('min-height',minHeight);
}
minHeight();

function progressBar(obj,type){
	var percentage = 0;
	var interval = setInterval(function () {
		if (percentage < 10000) {
			percentage++;
			var widthTemp = (percentage / 100).toFixed(1) + '%';
			$(obj).css('width', widthTemp);
			$(obj).parent('.progressBarBg').siblings('.progressData').find('span').css('left',widthTemp);
			$(obj).parent('.progressBarBg').siblings('.progressData').find('span').text(widthTemp);
		} else {
			clearInterval(interval);
			setTimeout(function () {
				if(type==true){
					$(obj).parent('.progressBarBg').siblings('.progressData').find('span').addClass('finish');
					$(obj).parent('.progressBarBg').siblings('.progressData').find('span').text('安装完成');
				}
				$(obj).removeClass('bg-ygradualChange').addClass('bg-yellow');
				$('#installing').addClass('hide');
				$('.nextStep').removeClass('hide');
			}, 300);
		}
	}, 1);
}
