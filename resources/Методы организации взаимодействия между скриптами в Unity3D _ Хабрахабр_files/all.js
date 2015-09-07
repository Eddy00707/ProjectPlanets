/**
 * В этом файле содержатся скрипты, которые используются на всех страницах профиля пользователя.
 */

/**
 * Функция для голосования за карму.
 */
function karma_vote(link, target_id, target_type, vote){
 	var link = $(link);
	if(link.hasClass('loading') || link.hasClass('voted')){
		// повторный клик. ни чо чо делать не надо :) пущай ждет ответ с сервера...
		
	}else{
		link.addClass('loading');
		$.post('/json/vote/',{ti:target_id, tt:target_type, v:vote},function(json){		
			if(json.messages == 'ok'){					
				var karma = $('.user_header .karma');
				
				// выделим отмеченный пункт.
				if(vote === 1){
					$('.vote_plus',karma).addClass('voted').attr('title','Вы проголосовали положительно.');
					$('.vote_minus',karma).attr('title','Понизить карму.');
					$('.vote_minus',karma).removeClass('voted');
				}else if(vote === -1){
					$('.vote_minus',karma).addClass('voted').attr('title','Вы проголосовали отрицательно.');
					$('.vote_plus',karma).attr('title','Повысить карму.');
					$('.vote_plus',karma).removeClass('voted');
				}
				
				// обновим кол-во голосов
				$('.votes', karma).text(json.votes_count);
				
				// обновим значение кармы
				$('.num', karma).text(json.score);
			}else{
				show_system_error(json);
			}
			link.removeClass('loading');
		},'json');
	}
	
	return false;
}


$(document).ready(function(){

	
	
	/** 
   * Подписаться/отписаться от юзера
   */
  $('#followUser').click(function(){
  	var link = $(this);
  			link.addClass('loading');
    var login = $(this).attr('data-login');
    $.post('/json/users/follow/', {'login':login}, function(json){
      if(json.messages =='ok'){
        $('#unfollowUser').removeClass('hidden');
        $('#followUser').addClass('hidden'); 
        var followers_count_string = '<a href="/users/'+login+'/subscription/followers/">'+json.followers_count_string+'</a>';
        $('#followers_count').html(followers_count_string);
      }else{
        show_system_error(json);
      }
      link.removeClass('loading');
    },'json');
    return false;
  });
  $('#unfollowUser').click(function(){
    var login = $(this).attr('data-login');
    var link = $(this);
  			link.addClass('loading');
  
      $.post('/json/users/unfollow/', {'login':login}, function(json){
        if(json.messages =='ok'){
          $('#unfollowUser').addClass('hidden');
          $('#followUser').removeClass('hidden');                
          var followers_count_string = json.followers_count > 0 ? '<a href="/users/'+login+'/subscription/followers/">'+json.followers_count_string+'</a>' : json.followers_count_string;
	        $('#followers_count').html(followers_count_string);
        }else{
          show_system_error(json);
        }
      	link.removeClass('loading');
      },'json');

    return false;
  });

	
	// обработчик кнопки "подарить инвайт"
	$('#give_invite_button').click(function(){
		var login = $(this).data('login');
		$.post('/json/users/send_invite/', {login: login}, function(json){
			if(json.messages == 'ok'){
				
				$.jGrowl('Вы подарили инвайт.<br> У вас осталось '+json.invites_string+'.', { theme: 'notice' });
				ajaxFormRedirect(json);
		 	}else{			 
				show_system_error(json);
		 	}
		});
		return false;
	});
	
})