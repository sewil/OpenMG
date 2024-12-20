  module "standard.s";  

  // 4/0 : 획득!!  
  // 5/0 : ? 박스  
  // 6/0 : 인기도  
  // 7/0 : 메소  
  // 8/0 : 경험치  
  // 8/0 : 친밀도  

  //로저의 사과  
  script "q1021s" {  
  	inven = target.inventory;  
  	qr = target.questRecord;  

  	if ( target.nGender == 0 ) {  
  		self.say( "E a�, cara~ Tudo legal? Haha! Eu sou #p2000# e passo para voc�s, novos viajantes, um mont�o de informa寤es." );  
  		self.say( "Voc� t� perguntando quem me fez fazer isto? Hahahaha! Eu mesmo! Eu quis fazer isto e simplesmente ser gentil com voc�s, novos viajantes." );  
  	}  
  	else if ( target.nGender == 1 ) {  
  		self.say( "Oi, pessoal~ Eu sou #p2000# e passo pra voc�s, pessoas novas em Maple, um mont�o de informa寤es." );  
  		self.say( "Sei que t� #Gocupado:ocupada#! Fica um tempo aqui comigo~ Posso te dar informa寤es muito �teis! Hahahaha!" );  
  	}  

  	v0 = self.askAccept( "Ent�o..... Deixa eu animar as coisas um pouco! Abracadabra~!");  
  	if ( v0 == 0 ) self.say( "N�o acredito que voc� acabou de recusar um cara simp�tico como eu!" );  
  	else {  
  		if ( inven.itemCount( 2010007 ) >= 1 ) {  
  			val3 = target.nHP / 2;  
  			target.incHP( -val3, 0 );  
  			qr.setState( 1021, 1 );  
  			self.say( "#GSurpreso:Surpresa#? Se o HP for para 0, voc� ter� um problema. Agora vou dar para voc� a #r#t2010007##k. Pode pegar. Voc� vai se sentir mais forte. Abra a Janela de Itens e d� dois cliques para usar.  Ei, � muito simples abrir a Janela de Itens. Simplesmente aperte #bI#k no seu teclado.#I" );  
  			self.say( "Pegue todas as #t2010007#s que eu te dei. Voc� poder� ver a barra de HP aumentando. Fale de novo comigo quando voc� recuperar 100% do HP#I" );  
  		} else {  
  			ret = inven.exchange( 0, 2010007, 1 );  
  			if ( ret == 0 ) self.say( "Ei, voc� t� carregando muitas coisas." );  
  			else {  
  				val2 = target.nHP / 2;  
  				target.incHP( -val2, 0 );  
  				qr.setState( 1021, 1 );  
  				self.say( "#GSurpreso:Surpresa#? Se o HP for para 0, voc� ter� um problema. Agora vou dar para voc� a #r#t2010007##k. Pode pegar. Voc� vai se sentir mais forte. Abra a Janela de Itens e d� dois cliques para usar.  Ei, � muito simples abrir a Janela de Itens. Simplesmente aperte #bI#k no seu teclado.#I" );  
  				self.say( "Pegue cada #t2010007# que eu te dei. Voc� poder� ver a barra de HP aumentando. Fale de novo comigo quando voc� recuperar 100% do HP#I" );  
  			}  
  		}  
  	}  
  }  

  script "q1021e" {  
  	inven = target.inventory;  
  	qr = target.questRecord;  
  	if ( inven.itemCount( 2010007 ) == 0 ) {  
  		if ( target.nHP == target.nMHP ) {  
  			file = "#fUI/UIWindow.img/QuestIcon/";  
  			self.say( "Se � f�cil usar o item? Simples, certo? Voc� pode configurar um #batalho#k no slot inferior direito. Haha, voc� n�o sabia disso, n�? Ah, e se voc� � #Gum:uma# aprendiz, o HP ir� automaticamente se recuperar com o passar do tempo. Bem, isso toma tempo, mas � uma das estrat�gias para aprendizes." );  
  			self.say( "Certo! Agora que voc� aprendeu bastante, vou te dar um presente. Voc� tem que ter para poder viajar no Mundo Maple, ent�o me agrade�a! Por favor, use em casos de emerg�ncia!" );  
  			self.say( "Certo, isso � tudo o que posso te ensinar. Sei que � triste, mas � hora de dizer adeus. Bem, se cuida e boa sorte #Gmeu:minha# #Gamigo:amiga#!\r\n\r\n" + file + "4/0#\r\n#v2010000# 3 #t2010000#\r\n#v2010009# 3 #t2010009#\r\n\r\n" + file + "8/0# 10 exp" );  

  			ret = inven.exchange( 0, 2010000, 3, 2010009, 3 );  
  			if ( ret == 0 ) self.say( "Puxa! Muitos itens no seu invent�rio....." );  
  			else {  
  				target.incEXP( 10, 0 );  
  				qr.setState( 1021, 2 );  
  				target.questEndEffect;  
  			}  
  		} else self.say( "Ei, seu HP ainda n�o se recuperou totalmente. Voc� pegou cada #t2010007# que eu te dei? Tem certeza?" );  
  	} else self.say( "Veja... Eu disse para voc� pegar cada #r#t2010007##k que eu te dei. Abra a Janela de Itens e clique na #baba USO#b. L� voc� ver� a #t2010007#, d� dois cliques para usar." );  
  }  

  script "q1028s" {  
  	qr = target.questRecord;  
	
  	self.say( "Ei, se voc� quiser ir para Ilha Victoria, eu posso te indicar o caminho a qualquer hora! � claro, s� se voc� puder pagar #b150 mesos#k. O qu�? Voc� tem a mensagem de #b#p12000##k??? Ah, voc� � a pessoa de quem ele falou. Certo, ent�o. Uma vez que voc� est� fazendo um favor para a Ilha Maple, eu te darei o caminho de gra�a." );  
  	v0 = self.askAccept( "E voc� sabe que, uma vez que voc� saia da Ilha Maple, voc� nunca mais poder� voltar para este lugar. Voc� chegar� em #b#m104000000##kde Victoria Island. � mais cheio de gente do que em Porto Sul, ent�o procure manter a cabe�a no jogo. N�o se preocupe, ser� muito f�cil para voc� encontrar.#b#p1002101##k" );  
  	if ( v0 == 0 ) self.say( "Oh, voc� precisa de mais tempo? Certo... Estarei esperando aqui at� voc� fazer tudo o que precisa na Ilha Maple." );  
  	else {  
  		qr.setState( 1028, 1 );  
  		registerTransferField( 104000000, "maple00" );  
  	}  
  }  

  //블록퍼스는 외계생물?  
  script "q3452e" {  
  	inven = target.inventory;  
  	qr = target.questRecord;  
  	if ( inven.itemCount( 4000099 ) >= 1 ) {  
  		if ( inven.itemCount( 4001125 ) >= 1 ) {  
  			file = "#fUI/UIWindow.img/QuestIcon/";  
  			self.say( "문어 열쇠고리는 구해 왔는가? 흐음... 귀엽게 생긴 물건이군. 하지만 이게 바로 지구를 위협하는 외계인의 정체를 밝힐 중요한 물건...잠깐!" );  
  			self.say( "자네가 가지고 있는 그 물건! 그것 좀 보여주게. 자네 손에 들고 있는 바로 그 설계도 말일세. 오~ 이런 물건을 어디서 구한건가? 이것만 있으면 블록퍼스에 대한 연구를 더 빨리 진행시킬 수 있겠어." );  
  			self.say( "뜻밖의 수확인걸. 좋아. 자네에게 특별한 선물을 하도록 하지. 분명 도움이 될거야. 하하하~\r\n\r\n" + file + "4/0#\r\n#v2040701# #t2040701# 1개\r\n\r\n" + file + "8/0# 16000 exp" );  

  			res = inven.exchange( 0, 4000099, -1, 4001125, -1, 2040701, 1 );  
  			if ( res == 0 ) self.say( "뭘 그렇게 많이 들고 다니는건가? 인벤토리에 빈 칸이 있는지 확인해 주게." );  
  			else {  
  				target.incEXP( 16000, 0 );  
  				qr.setState( 3452, 2 );  
  				target.questEndEffect;  
  				self.say( "블록퍼스와 외계인... 아무리 생각해도 뭔가 비슷하단 말이야. 아니, 문어와 외계인이 비슷한 걸지도... 흐음. 이것도 새로운 이론이군." );  
  			}  
  		} else {   
  			file = "#fUI/UIWindow.img/QuestIcon/";  
  			self.say( "문어 열쇠고리는 구해 왔는가? 흐음... 귀엽게 생긴 물건이군. 하지만 이게 바로 지구를 위협하는 외계인의 정체를 밝힐 중요한 물건이지. 정말 고맙네.\r\n\r\n" + file + "4/0#\r\n#v2000011# #t2000011# 50개\r\n\r\n" + file + "8/0# 8000 exp" );  
		
  			ret = inven.exchange(0, 4000099, -1, 2000011, 50 );  
  			if ( ret == 0 ) self.say( "뭘 그렇게 많이 들고 다니는건가? 인벤토리에 빈 칸이 있는지 확인해 주게." );  
  			else {  
  				target.incEXP( 8000, 0 );  
  				qr.setState( 3452, 2 );  
  				target.questEndEffect;  
  				self.say( "블록퍼스와 외계인... 아무리 생각해도 뭔가 비슷하단 말이야. 아니, 문어와 외계인이 비슷한 걸지도... 흐음. 이것도 새로운 이론이군." );  
  			}  
  		}  
  	} else self.say( "아직 문어 열쇠고리는 구하지 못한 모양이군. 그건 블록퍼스들이 가지고 있다네." );  
  }  

  //왕비의 비단 빼앗기  
  script "q3941s" {  
  	qr = target.questRecord;  
  	morphID = target.getMorphState;  
  	if ( morphID == 6 ) {  
  		self.askMenu( "...#p2101004#님 아니십니까? 오랜만입니다. 이번에 왕비님께서 애타게 찾으시던 비단을 다행히 구해놨습니다. 항상 그렇듯이 물건은 최고급... 그런데 왜 이렇게 땀을 흘리십니까?\r\n#L0##b(음성변조)아니, 그냥 태양이 더워서...#l" );  
  		self.askMenu( "아리안트가 언제는 안 더운 곳이었습니까? 항상 이랬지만 #p2101004#님은 더위를 안 타시는 줄 알았습니다만... 아니 점점 얼굴이 빨개지고 계십니다. 괜찮으십니까?\r\n#L0##b(음성변조)괘, 괜찮네. 걱정하지 말게...#l" );  
  		self.askMenu( "정말 괜찮으신 겁니까? 안 그래도 #p2101004#님은 허약체질 같으신데, 역시 약이라도 좀 드셔야 하는 것 아닙니까? 엘나스 쪽에서 구한, 감기약이 있는데 하나 사시겠습니까? 싸게 드리죠.\r\n#L0##b괘, 괜찮다니까 그러네!#l" );  
  		silk = self.askAccept( "괜찮으시다고요? 그런데 #p2101004#님 목소리가 평소하고 아주 많이 다릅니다? 정말 감기에 걸리신 것 아닙니까? 도무지 평소의 #p2101004#님 같지 않으십니다. 평소에는 항상 #t4010007#을 싸게 달라고 하셨는데... 이상하군요. 정말 #p2101004#님이십니까?" );  
		
  		if ( silk == 0 ) self.say( "#p2101004#님이 아니라고요? 아니 어딜 봐도 #p2101004#님이신데 도대체 무슨 말을 하시는 건지... 정말 많이 아프신 모양이군요. 비단은 괜찮아 지면 드릴 테니, 다시 찾아오시죠." );  
  		else {  
  			qr.setState( 3941, 1 );  
  			self.say( "평소의 #p2101004#님 같지 않으십니다. 원래 #p2101004#님이 이렇게 말이 적은 분이 아니신 걸로 아는데... 무슨 일이라도 있으신 겁니까? 헙. 얼굴이 점점 붉어지시는 게 아무래도 화가 나신 모양이군요. 죄송합니다. 어서 가서 비단을 가져오겠습니다. 잠시만 기다려 주세요." );  
  		}  
  	} else self.say( "날씨가 정말 덥죠? 하지만 또 더워야 진짜 사막이라고 할 수 있지 않겠어요? 그나저나 #p2101004#님은 언제 오시려나?" );  
  }  

  script "q3941e" {  
  	qr = target.questRecord;  
  	inven = target.inventory;  
  	morphID = target.getMorphState;  
  	file = "#fUI/UIWindow.img/QuestIcon/";  
  	if ( morphID == 6 ) {  
  		self.say( "자 여기 있습니다. 조심해서 가져가십시오. 이 비단, 구하기 무척 힘든 물건입니다. 혹시 조금이라도 찢어진 곳이 있으면 당장 왕비님께서 #p2101004#님을 감옥에 가두고 말 것입니다.\r\n\r\n" + file + "4/0#\r\n#v4031571# #t4031571# 1개\r\n\r\n" );  

  		ros = inven.exchange( 0, 4031571, 1 );  
  		if ( ros == 0 ) self.say( "인벤토리에 짐이 많아서 비단을 넣으드릴 수가 없는걸요? 중요한 물건을 운반하시는데 조금만 자리를 비워보세요." );  
  		else {  
  			qr.setState( 3941, 2 );  
  			target.questEndEffect;  
  		}  
  	} else self.say( "날씨가 정말 덥지 않아요? 목이 바짝바짝 타는 군요." );  
  }  

  //아딘의 모래그림단  
  script "q3933s" {  
  	qr = target.questRecord;  
  	quest = FieldSet( "Adin" );  

  	self.say( "네가 이렇게 강할 줄 몰랐어. 너 정도면 모래그림단원이 될 수 있을지도 모르겠다는 생각이 드는걸? 모래그림단원에게 가장 중요한 건 강함이고, 넌 충분히 강한 것 같거든. 하지만 역시 한 번만 더 시험을 해보고 싶은데, 어때? 괜찮겠어? " );  
  	adin1 = self.askAccept( "진짜 너의 강함을 확인하려면 역시 몸으로 부딪혀 보는 수밖에 없겠지? 너와 대련을 해보고 싶어. 걱정 말라구. 너를 해치고 싶지는 않아. 내 분신으로 널 상대해주지. 지금 당장 대련에 들어가도 괜찮겠어?" );  
  	if ( adin1 == 0 ) self.say( "마음의 준비가 필요한건가? 너무 긴장하지는 말라구." );  
  	else {  
  		self.say( "좋아. 자신만만하군." );  
  		setParty = FieldSet( "Adin" );  
  		adin2 = setParty.enter( target.nCharacterID, 0 );  
  		qr.setState( 3933, 1 );  

  		if ( adin2 != 0 ) self.say( "아... 잠시만 기다려 주게. 지금 누군가가 대련장을 쓰고 있는 것 같아. 잠시 후에 다시 찾아와 주게." );  
  	}  
  }  

  // 사막으로... : 아리안트로 5회 보내주기  
  script "q2127e" {  
  	qr = target.questRecord;  
  	info = qr.get( 7060 );  
  	if ( info == "" ) {  
  		qr.set( 7060, "0" );  
  		info = qr.get( 7060 );  
  	}  

  	lv =  target.nLevel;  

  	if ( info == "0" ) {  
  		a1 = self.askAccept( "떠날 준비는 다 되었나? 처음 가는 거라서 긴장을 한 것 같군. 걱정말게. 자네는 잘 할 수 있을 거야. 내 능력으로 앞으로 다섯 번은 그곳으로 보내 줄 수 있네. 지금 이동하겠나?" );  
  		if ( a1 == 0 ) self.say( "준비가 되면 다시 찾아오게." );  
  		else {  
  			self.say( "잘 다녀오게." );  
  			registerTransferField( 260000200, "" );  
  			qr.set( 7060, "1" );  
  		}  
  	} else if ( info == "1" ) {  
  		a1 = self.askAccept( "또 만나게 됐군. 지난 번에 갔을 때는 어땠나? 다시 가는 것을 보니 그 곳에서 하던 일이 있나보지? 자네에겐 네 번의 기회가 남아 있네. 지금 이동하겠나?" );  
  		if ( a1 == 0 ) self.say( "준비가 되면 다시 찾아오게." );  
  		else {  
  			self.say( "잘 다녀오게." );  
  			registerTransferField( 260000200, "" );  
  			qr.set( 7060, "2" );  
  		}  
  	} else if ( info == "2" ) {  
  		a1 = self.askAccept( "자주 보게 되는군. 무척 바쁜 모양이야? 자네에겐 세 번의 기회가 남아 있네. 지금 이동 하겠나?" );  
  		if ( a1 == 0 ) self.say( "준비가 되면 다시 찾아오게." );  
  		else {  
  			self.say( "잘 다녀오게." );  
  			registerTransferField( 260000200, "" );  
  			qr.set( 7060, "3" );  
  		}  
  	} else if ( info == "3" ) {  
  		a1 = self.askAccept( "사막은 매력적인 곳인가 보지? 그러고 보니 얼굴도 검게 그을린 것 같군. 자네에겐 두 번의 기회가 남아 있네. 지금 이동 하겠나?" );  
  		if ( a1 == 0 ) self.say( "준비가 되면 다시 찾아오게." );  
  		else {  
  			self.say( "잘 다녀오게." );  
  			registerTransferField( 260000200, "" );  
  			qr.set( 7060, "4" );  
  		}  
  	} else if ( info == "4" ) {  
  		a1 = self.askAccept( "오늘도 그곳으로 가는건가? 이름이 뭐라고 했지? 아리안트라고 했었나? 자네를 보니 나도 한 번쯤 가보고 싶어지는군. 자네에겐 한 번의 기회가 남아 있네. 지금 이동하겠나?" );  
  		if ( a1 == 0 ) self.say( "준비가 되면 다시 찾아오게." );  
  		else {  
  			self.say( "잘 다녀오게." );  
  			registerTransferField( 260000200, "" );  
  			qr.set( 7060, "5" );  
  			qr.setState( 2127, 2 );  
  			target.questEndEffect;  
  		}  
  	}  
  }  

  // 모래그림단의 보급품 : 18렙 이상 29렙 이하  
  script "q2124e" {  
  	qr = target.questRecord;  
  	inven = target.inventory;  
  	val = qr.getState( 3937 );  
  	file = "#fUI/UIWindow.img/QuestIcon/";  

  	if ( val == 2 ) {  
  		self.say( "아! 누군가 했더니 오랜만이야. 이번엔 보급품 운반을 맡았나보지? 꽤 중요한 임무였는데. 수고했어.\r\n\r\n" + file + "4/0#\r\n#v2030000# #t2030000# 5개\r\n#v2022155# #t2022155# 5개\r\n\r\n" + file + "8/0#\r\n2000 exp" );  

  		s1 = inven.exchange( 0, 4031619, -1, 2030000, 5, 2022155, 5 );  
  		if ( s1 == 0 ) self.say( "소비창에 빈 칸이 있는지 확인해 주세요." );  
  		target.incEXP( 2000, 0 );  
  		qr.setState( 2124, 2 );  
  		target.questEndEffect;  
  	}  
  	else if ( val == 0 or val == 1 ) {  
  		self.say( "수고했어요. 보는 눈이 많으니까 그만 가보도록해요.\r\n\r\n" + file + "4/0#\r\n#v2030000# #t2030000# 5개\r\n#v2022155# #t2022155# 5개\r\n\r\n" + file + "8/0#\r\n2000 exp" );  

  		s1 = inven.exchange( 0, 4031619, -1, 2030000, 10 );  
  		if ( s1 == 0 ) self.say( "소비창에 빈 칸이 있는지 확인해 주세요." );  
  		target.incEXP( 2000, 0 );  
  		qr.setState( 2124, 2 );  
  		target.questEndEffect;  
  	}  
  }  

  // 모래그림단의 보급품 :  30렙 이상  
  script "q2126e" {  
  	qr = target.questRecord;  
  	inven = target.inventory;  
  	val = qr.getState( 3937 );  
  	file = "#fUI/UIWindow.img/QuestIcon/";  

  	if ( val == 2 ) {  
  		self.say( "아! 누군가 했더니 오랜만이야. 이번엔 보급품 운반을 맡았나보지? 꽤 중요한 임무였는데. 수고했어.\r\n\r\n" + file + "4/0#\r\n#v2030000# #t2030000# 5개\r\n#v2022155# #t2022155# 5개\r\n\r\n" + file + "8/0#\r\n2000 exp" );  

  		s1 = inven.exchange( 0, 4031624, -1, 2030000, 5, 2022155, 5 );  
  		if ( s1 == 0 ) self.say( "소비창에 빈 칸이 있는지 확인해 주세요." );  
  		else {  
  			target.incEXP( 2000, 0 );  
  			qr.setState( 2126, 2 );  
  			target.questEndEffect;  
  		}  
  	}  
  	else if ( val == 0 or val == 1 ) {  
  		self.say( "수고했어요. 보는 눈이 많으니까 그만 가보도록해요.\r\n\r\n" + file + "4/0#\r\n#v2030000# #t2030000# 10개\r\n\r\n" + file + "8/0#\r\n2000 exp" );  

  		s1 = inven.exchange( 0, 4031624, -1, 2030000, 10 );  
  		if ( s1 == 0 ) self.say( "소비창에 빈 칸이 있는지 확인해 주세요." );  
  		else {  
  			target.incEXP( 2000, 0 );  
  			qr.setState( 2126, 2 );  
  			target.questEndEffect;  
  		}  
  	}  
  }  

  script "thief_in1" {  
  	field = Field( 260010401 );  
	
  	target.enforceNpcChat( 2103008 );  
  }  

  script "thief_in2" {  
  	answer = self.askText( "동굴의 문을 열고 싶다면 암호를 말해라...", "", 0, 11 );  
  	if ( answer == "열려라 참깨" or answer == "열려라참깨" ) {  
  		target.message( "암호를 말하자 신비한 힘이 동굴 안으로 인도한다." );  
  		registerTransferField( 260010402, "center00" );  
  	} else target.message ( "동굴 문은 꿈쩍도 하지 않는다." );  
  }  

  //늙은 도라지 모으기  
  script "q3833e" {  
  	inven = target.inventory;  
  	qr = target.questRecord;  
  	nItem = inven.itemCount( 4000294 );  
  	if ( nItem >= 1000 ) {  
  		file = "#fUI/UIWindow.img/QuestIcon/";  
  		self.say( "허... 허허허허. 자네는 인간인가?! 이렇게 많이 구해 오다니, 도대체 늙은 도라지를 몇 마리나 학살한 겐가...?! 험. 아무튼 고맙네. 자네가 구한 #b#t4000294# " + nItem + "개#k를 어서 나에게 주게.\r\n\r\n" + file + "4/0#\r\n" + file + "5/0#\r\n\r\n" + file + "8/0# 54000 exp" );  
  		res = inven.exchange( 0, 4000294, -nItem, 2000005, 50, 2040501, 1 );  
  		if ( res == 0 ) self.say( "뭘 그렇게 많이 들고 다니는건가? 인벤토리에 빈 칸이 있는지 확인해 주게." );  
  		else {  
  			target.incEXP( 54000, 0 );  
  			qr.setState( 3833, 2 );  
  			target.questEndEffect;  
  			self.say( "이 정도면 태상이 기절하지 않을까..." );  
  		}  
  	}  
  	else  if ( nItem >= 900 and nItem < 1000 ) 	{  
  		file = "#fUI/UIWindow.img/QuestIcon/";  
  		self.say( "아니... 정말 이게 모두 자네가 구한 것인가? 괴, 굉장하군... 이 정도까지 구하다니... 자네는 진정 굉장한 모험가일세. 정말 훌륭해! 이 정도면 이런 걸 줘도 아깝지 않지. 자네가 구한 #b#t4000294# " + nItem + "개#k를 어서 나에게 주게.\r\n\r\n" + file + "4/0#\r\n" + file + "5/0#\r\n\r\n" + file + "8/0# 54000 exp" );  
  		ret = inven.exchange(0, 4000294, - nItem, 2020013, 50, 2040502, 1 );  
  		if ( ret == 0 ) self.say( "뭘 그렇게 많이 들고 다니는건가? 인벤토리에 빈 칸이 있는지 확인해 주게." );  
  		else {  
  			target.incEXP( 54000 , 0 );  
  			qr.setState( 3833, 2 );  
  			target.questEndEffect;  
  			self.say( "이렇게 많은 걸 무슨 수로 무릉까지 가져간다...?" );  
  		}  
  	}  
  	else  if ( nItem >= 700 and nItem < 900 ) 	{  
  		file = "#fUI/UIWindow.img/QuestIcon/";  
  		self.say( "호오... 정말 굉장하군. 여기까지 모으는 게 쉽지 않았을 텐데. 정말 고맙네. 자네가 구한 #b#t4000294# " + nItem + "개#k를 어서 나에게 주게.\r\n\r\n" + file + "4/0#\r\n" + file + "5/0#\r\n\r\n" + file + "8/0# 54000 exp" );  
  		ret = inven.exchange(0, 4000294, - nItem, 2000004, 50, 2040500, 1 );  
  		if ( ret == 0 ) self.say( "뭘 그렇게 많이 들고 다니는건가? 인벤토리에 빈 칸이 있는지 확인해 주게." );  
  		else {  
  			target.incEXP( 54000 , 0 );  
  			qr.setState( 3833, 2 );  
  			target.questEndEffect;  
  			self.say( "흠. 잘못하다간 도라지를 배에 모두 싣기도 어렵겠는걸." );  
  		}  
  	}  
  	else  if ( nItem >= 500 and nItem < 700 ) 	{  
  		file = "#fUI/UIWindow.img/QuestIcon/";  
  		self.say( "오오... 많군. 이 정도면 태상도 할 말 없겠지. 자네 보기보다 제법이군. 자네가 구한 #b#t4000294# " + nItem + "개#k를 어서 나에게 주게.\r\n\r\n" + file + "4/0#\r\n" + file + "5/0#\r\n\r\n" + file + "8/0# 54000 exp" );  
  		ret = inven.exchange(0, 4000294, - nItem, 2020013, 50 );  
  		if ( ret == 0 ) self.say( "뭘 그렇게 많이 들고 다니는건가? 인벤토리에 빈 칸이 있는지 확인해 주게." );  
  		else {  
  			target.incEXP( 54000 , 0 );  
  			qr.setState( 3833, 2 );  
  			target.questEndEffect;  
  			self.say( "도라지를 배까지 옮기는 것도 쉽지 않겠는걸. 황선장에게 옮겨달라고 부탁해야겠군..." );  
  		}  
  	}  
  	else  if ( nItem >= 300 and nItem < 500 ) 	{  
  		file = "#fUI/UIWindow.img/QuestIcon/";  
  		self.say( "흠... 뭐 이 정도면 당분간 태상도 별 말 않겠지. 고맙네. 자네가 구한 #b#t4000294# " + nItem + "개#k를 어서 나에게 주게.\r\n\r\n" + file + "4/0#\r\n" + file + "5/0#\r\n\r\n" + file + "8/0# 51000 exp" );  
  		ret = inven.exchange(0, 4000294, - nItem, 2020012, 50 );  
  		if ( ret == 0 ) self.say( "뭘 그렇게 많이 들고 다니는건가? 인벤토리에 빈 칸이 있는지 확인해 주게." );  
  		else {  
  			target.incEXP( 51000 , 0 );  
  			qr.setState( 3833, 2 );  
  			target.questEndEffect;  
  			self.say( "이 정도면 태상이 만족하겠지." );  
  		}  
  	}  
  	else  if ( nItem >= 200 and nItem < 300 ) 	{  
  		file = "#fUI/UIWindow.img/QuestIcon/";  
  		self.say( "좋아. 이 정도면 그럭저럭 태상을 달래놓을 수 있을 것 같군. 감사하지. 자네가 구한 #b#t4000294# " + nItem + "개#k를 어서 나에게 주게.\r\n\r\n" + file + "4/0#\r\n" + file + "5/0#\r\n\r\n" + file + "8/0# 48000 exp" );  
  		ret = inven.exchange(0, 4000294, - nItem, 2001001, 50 );  
  		if ( ret == 0 ) self.say( "뭘 그렇게 많이 들고 다니는건가? 인벤토리에 빈 칸이 있는지 확인해 주게." );  
  		else {  
  			target.incEXP( 48000 , 0 );  
  			qr.setState( 3833, 2 );  
  			target.questEndEffect;  
  			self.say( "자아, 그럼 도라지를 잘 말려봐야겠군. 약재로 쓰려면 말리는 것도 중요하지." );  
  		}  
  	}  
  	else  if ( nItem >= 100 and nItem < 200 ) 	{  
  	file = "#fUI/UIWindow.img/QuestIcon/";  
  	self.say( "흐음. 좀 모자라긴 하지만 뭐, 당장 급한 건 이쪽이니 어쩔 수 없지. 자네가 구한 #b#t4000294# " + nItem + "개#k를 어서 나에게 주게.\r\n\r\n" + file + "4/0#\r\n" + file + "5/0#\r\n\r\n" + file + "8/0# 45000 exp" );  
  	ret = inven.exchange(0, 4000294, - nItem, 2020008, 50 );  
  	if ( ret == 0 ) self.say( "뭘 그렇게 많이 들고 다니는건가? 인벤토리에 빈 칸이 있는지 확인해 주게." );  
  	else {  
  		target.incEXP( 45000 , 0 );  
  		qr.setState( 3833, 2 );  
  		target.questEndEffect;  
  		self.say( "쯧. 태상이 한동안 투덜대겠군.." );  
  		}  
  	}  
  	else  if ( nItem >= 50 and nItem < 100 ) 	{  
  	file = "#fUI/UIWindow.img/QuestIcon/";  
  	self.say( "에잉. 겨우 이 정도인가? 아주 적은 건 아니지만 만족스럽진 않군. 자네가 구한 #b#t4000294# " + nItem + "개#k를 어서 나에게 주게.\r\n\r\n" + file + "4/0#\r\n" + file + "5/0#\r\n\r\n"  + file + "8/0# 10000 exp" );  
  	ret = inven.exchange(0, 4000294, - nItem, 2020007, 50 );  
  	if ( ret == 0 ) self.say( "뭘 그렇게 많이 들고 다니는건가? 인벤토리에 빈 칸이 있는지 확인해 주게." );  
  	else {  
  		target.incEXP( 10000 , 0 );  
  		qr.setState( 3833, 2 );  
  		target.questEndEffect;  
  		self.say( "약탈만 아니었더라도... 이런 식으로 구할 필요 없을 텐데." );  
  		}  
  	}  
  	else  if ( nItem >= 10 and nItem < 50 ) 	{  
  	file = "#fUI/UIWindow.img/QuestIcon/";  
  	self.say( "겨우 이게 단가? 허음... 자네 능력이 이것뿐이 안 된다니 뭐라 할 수는 없겠지만... 쯧쯧쯧. 젊은이가 이래서야 원. 자네가 구한 #b#t4000294# " + nItem + "개#k를 어서 나에게 주게.\r\n\r\n" + file + "4/0#\r\n" + file + "5/0#\r\n\r\n" + file + "8/0# 1000 exp" );  
  	ret = inven.exchange(0, 4000294, - nItem, 2022144, 10 );  
  	if ( ret == 0 ) self.say( "뭘 그렇게 많이 들고 다니는건가? 인벤토리에 빈 칸이 있는지 확인해 주게." );  
  	else {  
  		target.incEXP( 1000 , 0 );  
  		qr.setState( 3833, 2 );  
  		target.questEndEffect;  
  		self.say( "이래가지고는 태상을 볼 면목이 없군..." );  
  		}  
  	}  
  	else  if ( nItem >= 1 and nItem < 10 ) 	{  
  	file = "#fUI/UIWindow.img/QuestIcon/";  
  	self.say( "...뭐, 그래. 어쨌든 가져다 주긴 하는구만. 자네가 구한 #b#t4000294# " + nItem + "개#k를 어서 나에게 주게.\r\n\r\n" + file + "4/0#\r\n" + file + "5/0#\r\n\r\n"  + file + "8/0# 10 exp" );  
  	ret = inven.exchange(0, 4000294, - nItem, 2000000, 1 );  
  	if ( ret == 0 ) self.say( "뭘 그렇게 많이 들고 다니는건가? 인벤토리에 빈 칸이 있는지 확인해 주게." );  
  	else {  
  		target.incEXP( 10 , 0 );  
  		qr.setState( 3833, 2 );  
  		target.questEndEffect;  
  		self.say( "한동안 태상과 연락을 하지 않는 게 좋을 듯 하군..." );  
  		}							  
  	} else self.say( "아직 100년 묵은 도라지는 구하지 못한 모양이군. 그건 늙은 도라지를 잡아 얻을 수 있다네." );  
  }  

  //실종연금, 그리고 실종된 연금술사  
  script "q3314e" {  
  	inven = target.inventory;  
  	qr = target.questRecord;  
  	ret = target.checkCondition;  
  	len = length( ret );  
  	file = "#fUI/UIWindow.img/QuestIcon/";  

  	for ( i = 0..len ) {  
  		con = integer( substring( ret, i, 1 ));  
  		if ( con == 6 ) {  
  			a1 = self.askMenu( "후후후후후.... 안색이 창백해진 걸 보니 역시 효과가 있군. 이번 실험은 성공이야! 으하하하! 역시 로이드를 해치울 정도로 튼튼한 녀석에게는 써도 괜찮군!\r\n#L0##b(역시 인체실험이었나!)#k#l" );  

  			if ( a1 == 0 ) {  
  				a2 = self.askMenu( "무척 놀란 표정인걸? 그렇게 걱정할 것 없어. 그리 위험한 약은 아니야... 아니, 위험한 약이지만 해독제는 있으니까... 후후후후....\r\n#L0##b(병 주고 약 줘봤자 소용 없어!)#k#l" );  

  				if ( a2 == 0 ) {  
  					a3 = self.askMenu( "이것으로 임의로 인체의 상태를 변경할 수 있게 되었군. 이제 보다 생명연금이 쉬워질 거야. 이걸로, 이제 그 녀석의 바램을 이뤄줄 수 있을지도 모르겠군...\r\n#L0##b그 녀석이요?#k#l" );  

  					if ( a3 == 0 ) {  
  						self.say( "그래... 그 녀석. 생명연금 쪽에서는 최고인 녀석이었지. 누구보다 훌륭한 실력을 가진 녀석이었는데... 녀석만 있다면 이런 연구는 금방 해결했겠지. 하지만 어쩔 수 없어... 녀석은 이미 실종되어 버렸으니까...\r\n\r\n" + file + "5/0#\r\n\r\n" + file + "8/0# 40300 exp" );  

  						a = random( 0, 20 );  
  						if ( a == 0 ) nItem = 2022199;  
  						else if ( a >= 1 and a < 6 ) nItem = 2022224;  
  						else if ( a >= 6 and a < 11 ) nItem = 2022225;  
  						else if ( a >= 11 and a < 16 ) nItem = 2022226;  
  						else if ( a >= 16 and a <= 20 ) nItem = 2022227;  

  						rat = inven.exchange( 0, 2050004, 10, nItem, 20 );  

  						if ( rat == 0 ) self.say( "소비창이 가득찬 것은 아닌가? 확인해 보게." );  
  						else {  
  							target.incEXP( 12500, 0 );  
  							qr.setState( 3314, 2 );  
  							target.questEndEffect;  

  							self.say( "녀석이 왜 실종되었는지는 아무도 몰라. 언제부턴가 녀석은 조급해했고, 사람들 몰래 알 수 없는 연구를 하기 시작했어. 아무리 물어도 어떤 연구인지 말하지 않았어. 녀석은 반쯤 미친 듯했지. 연구, 연구, 연구... 쉴새없이 연구만 했지. 생명연금에 관한... 그리고 결국, #b그 사건#k이 벌어졌지..." );  
  							self.say( "연금술사들의 마을이라는 마가티아에서도, 그 정도의 대형 폭발은 단 한 번도 없었어... 녀석이 어떤 실험을 했는지, 짐작조차 가지 않아. 도대체 어떤 무시무시한 것을 연구한 것일까... 녀석의 집을 조사했으니 협회장은 뭔가 알고 있을 텐데 아무 것도 말해주지 않아..." );  
  							self.say( "이 연구도 처음에는 녀석과 합작한 것이었어. 하지만 그 녀석은 사라졌고 더 이상 연구를 진행하기는 어려워졌지. 약에는 자신이 있는 편이지만 그래도 역시 쉽지 않아. 녀석이 하던 것이니 계속 하고 있기는 하지만... 녀석은 도대체 왜 인체의 상태를 변경하는 연구를 한 것일까...?" );  
  							self.say( "녀석은 아직 살아있을 거야. 그 녀석에게는, 그래야 할 이유가 있으니까." );  
  							end;  
  						}  
  					}  
  				}  
  			}  
  		}  
  	} self.say( "...아직도 약을 먹지 않은 모양이군. 이 러셀론을 믿지 못한다는 건가? 알카드노 선배로서 자네에게 모범만을 보였다고 생각해 왔는데...." );  
  }  

  //알카드노의 망토 다시 받기  
  script "q3306s" {  
  	qr = target.questRecord;  
  	inven = target.inventory;  
  	wear = target.isWear( 1102136 );  
  	val1 = qr.getState( 3347 );  
  	val2 = qr.getState( 3348 );  
  	val3 = qr.getState( 3349 );  

  	if ( val1 == 2 or val2 == 2 or val3 == 2 ) self.say( "알카드노로써 누릴 수 있는 혜택을 이미 다 받지 않았나. 유감이지만 더 이상 알카드노의 망토를 재지급해 줄 수 없네." );  
  	else {  
  		if ( wear == 0 ) {  
  			a1 = self.askAccept( "본 기억이 있는 얼굴인 걸 보니 자네는 분명 알카드노 소속의 연금술사인 모양인데... 왜 알카드노의 망토를 하고 있지 않은 건가? 알카드노 소속이라면 누구나 하고 있어야 한다고 말했는데... 뭐? 망토를 잃어버렸다고...? 더 이상 알카드노가 되고 싶지 않다는 말인가? 흐음... 그건 아닌 모양이군. 그럼 망토를 다시 지급할 테니 망토를 받을 텐가?" );  

  			if ( a1 == 0 ) self.say( "싫다면 할 수 없지. 망토가 없다고 알카드노 소속이 아니게 되는 것은 아니지만... 누구도 자네를 알카드노로 생각하지 않을 것이니 명심하게." );  
  			else {  
  				qr.setState( 3306, 1 );  
  				self.say( "알카드노에 처음 들어올 때가 망토를 그냥 지급했지만, 재지급할 때는 직접 재료를 모아와야 하네. 재료는 #b#t4000021# 10개#k와 #b#t4021006# 5개#k, 연성하는 사람에게 지급할 #b10000메소#k라네. 그럼 잊지 말고 가져오게." );  
  			}  
  		} else self.say( "알카드노의 망토를 이미 가지고 있는걸 보니 새로운 망토가 필요하지 않겠군." );  
  	}  
  }  

  //제뉴미스트의 망토 다시 받기  
  script "q3305s" {  
  	qr = target.questRecord;  
  	inven = target.inventory;  
  	wear = target.isWear( 1102135 );  
  	val1 = qr.getState( 3347 );  
  	val2 = qr.getState( 3348 );  
  	val3 = qr.getState( 3349 );  

  	if ( val1 == 2 or val2 == 2 or val3 == 2 ) self.say( "제뉴미스트로써 누릴 수 있는 혜택을 이미 다 받은 것 같은데... 유감이지만 더 이상 제뉴미스트의 망토를 재지급해 줄 수 없네." );  
  	else {  
  		if ( wear == 0 ) {  
  			a1 = self.askAccept( "으흠? 자네는 얼마 전에 제뉴미스트 소속이 된 연금술사 아닌가? 그런데 제뉴미스트의 망토는 어디에... 이런. 망토를 잃어버린 모양이군. 망토는 자랑스러운 제뉴미스트 학파임을 증명하는 소중한 것. 그런데 그런 것을 잃어버리다니... 정신이 있는 겐가? 매우 불쾌하군. 자네 같은 사람에게는 다시 망토를 주고 싶지 않지만... 어쩔 수 없지. 망토를 다시 받겠는가?" );  

  			if ( a1 == 0 ) self.say( "싫다면 하는 수 없지. 자네는 더 이상 제뉴미스트 소속이 아닐세." );  
  			else {  
  				qr.setState( 3305 , 1 );  
  				self.say( "한 번 더 기회를 주도록 하지. 제뉴미스트의 망토를 만들고 싶다면, #b#t4000021# 10개#k와 #b#t4021003##k 5개, 연성하는 사람을 위한 수고비 #b10000메소#k를 가져오게." );  
  			}  
  		} else self.say( "알카드노의 망토를 이미 가지고 있는걸 보니 새로운 망토가 필요하지 않겠군." );  
  	}  
  }  

  // 파웬이 알고 있는 것  
  script "q3320s" {  
  	qr = target.questRecord;  
  	self.say( "오오! 자네 왔군! 반갑네, 반가워! 자네 덕분에 요즘은 심심하지 않다네~ ...응? 뭐라고? 여기서 연구하던 연금술사가 누구냐고? 음... 그의 이름을 알기는 했는데...  " );  
  	self.say( "뭐더라? 뭐더라... 뭐더라아... 아아! 도무지 떠오르지 않는군. 혹시 그 사람 자네에게 중요한 사람인가? 웬만하면 그냥 잊어버리면... 안된다고? 그럼 어쩐다아... " );  
  	v0 = self.askAccept( "에잇! 모르겠다. 그냥 자네가 직접 보게!" );  
  	if ( v0 == 0 ) self.say( "엥? 싫은가? 자네가 싫다면 하는 수 없지만... 그럼 여기서 연구하던 연금술사는 알려줄 수가 없는데?" );  
  	else {  
  		qr.setState( 3320, 1 );  
  		registerTransferField( 926120200, "out00" );  
  	}  
  }  

  // 실종된 연금술사, 드랭  
  script "q3321s" {  
  	qr = target.questRecord;  
  	self.say( "...어째서 이런 곳에 오신 건지 모르지만... 연금술사의 실험실은 그리 즐거운 곳이 아닙니다. 연금술사가 아닌 사람의 눈에는 무척 지루하다더군요. 하긴... 그녀야 요정이니 더 재미없어 보일지도 모르겠군요...  " );  
  	self.say( "그녀가 누구냐고요? 그녀는... 제 아내입니다. 그러고 보니 그녀의 얼굴을 못 본지도 꽤 오래 되었군요... 딸 아이의 얼굴이 가물가물할 정도이니... 그녀가 무척 화를 내겠군요. 물론 상냥한 그녀는 곧 용서해 줄 테지만요... " );  
  	self.say( "...하지만 어쩔 수 없지요. 이 연구를 마치기 전까지 그녀의 얼굴을 보지 않겠다고 결심했으니까요. 무척 보고 싶지만... 연구를 마치기 전까지는... 연구만 마치면 영원히 #b#p2111004##k의 얼굴을 볼 수 있을 겁니다." );  
  	self.say( "그러고 보니 #b펜던트#k를 아직도 그녀에게 선물하지 못했군요. 그녀에게 들킬까봐 #b액자 뒤#k에 숨겨 놓기까지 했었는데... 그녀의 얼굴을 볼 수 없으니 선물조차 할 수 없네요. 언제쯤이면 그녀를 볼 수 있을까요... " );  
  	v0 = self.askAccept( "...쓸데없는 이야기가 너무 길어졌군요. 죄송합니다만, 연구를 계속 해야 해서... 그만 이 연구실에서 나가주십시오." );  
  	if ( v0 == 0 ) self.say( "무례하신 분이군요..." );  
  	else {  
  		qr.setState( 3321, 1 );  
  		registerTransferField( 261020401, "" );  
  	}  
  }  

  //제뉴미스트 협회장의 시험  
  script "q3301e" {  
  	qr = target.questRecord;  
  	inven = target.inventory;  

  	j1 = 0; j2 = 0; j3 = 0; j4 = 0; j5 = 0; j6 = 0; j7 = 0; j8 = 0; j9 = 0; j10 = 0; j11 = 0;  

  	if ( inven.itemCount( 4020000 ) >= 2 ) j1 = 1;  
  	if ( inven.itemCount( 4020001 ) >= 2 ) j2 = 1;  
  	if ( inven.itemCount( 4020002 ) >= 2 ) j3 = 1;  
  	if ( inven.itemCount( 4020003 ) >= 2 ) j4 = 1;  
  	if ( inven.itemCount( 4020004 ) >= 2 ) j5 = 1;  
  	if ( inven.itemCount( 4020005 ) >= 2 ) j6 = 1;  
  	if ( inven.itemCount( 4020006 ) >= 2 ) j7 = 1;  
  	if ( inven.itemCount( 4020007 ) >= 2 ) j8 = 1;  
  	if ( inven.itemCount( 4020008 ) >= 2 ) j9 = 1;  
  	if ( inven.itemCount( 4010004 ) >= 2 ) j10 = 1;  
  	if ( inven.itemCount( 4010006 ) >= 2 ) j11 = 1;  

  	str = "";  
  	if ( j1 == 1 ) str = str + "\r\n#L0##b #t4020000##k#l";  
  	if ( j2 == 1 ) str = str + "\r\n#L1##b #t4020001##k#l";  
  	if ( j3 == 1 ) str = str + "\r\n#L2##b #t4020002##k#l";  
  	if ( j4 == 1 ) str = str + "\r\n#L3##b #t4020003##k#l";  
  	if ( j5 == 1 ) str = str + "\r\n#L4##b #t4020004##k#l";  
  	if ( j6 == 1 ) str = str + "\r\n#L5##b #t4020005##k#l";  
  	if ( j7 == 1 ) str = str + "\r\n#L6##b #t4020006##k#l";  
  	if ( j8 == 1 ) str = str + "\r\n#L7##b #t4020007##k#l";  
  	if ( j9 == 1 ) str = str + "\r\n#L8##b #t4020008##k#l";  
  	if ( j10 == 1 ) str = str + "\r\n#L9##b #t4010004##k#l";  
  	if ( j11 == 1 ) str = str + "\r\n#L10##b #t4010006##k#l";  

  	if ( str == "" ) {  
  		self.say( "뭐야, 댓가로 줄만한 원석을 가지고 있지 않잖아? 댓가가 없으면 거래도 없지." );  
  		end;  
  	}  

  	ret = self.askMenu( "오호... 표정을 보아하니 거래할 준비가 된 모양이군. 그렇게까지 해서 제뉴미스트에 가입하고 싶다니... 이해할 수 없지만, 뭐 좋아. 댓가로 무엇을 주겠어?\r\n" + str );  
  	if ( ret == 0 ) {  
  		res = inven.exchange( 0, 4020000, -2 );  

  		if ( res == 0 ) self.say( "뭐야! #t4020000# 2개를  준비하지 못했잖아? 이봐. 거래를 하고 싶다면 준비를 제대로 해야지!" );  
  		else {  
  			qr.setState( 3301, 2 );  
  			target.questEndEffect;  
  			self.say( "그럼 잠시만 기다려. 네가 제뉴미스트 협회장의 시험을 통과하도록 만들어줄, 그 물건을 구해 놓을 테니." );  
  		}  
  	} else if ( ret == 1 ) {  
  		res = inven.exchange( 0, 4020001, -2 );  

  		if ( res == 0 ) self.say( "뭐야! #t4020001# 2개를  준비하지 못했잖아? 이봐. 거래를 하고 싶다면 준비를 제대로 해야지!" );  
  		else {  
  			qr.setState( 3301, 2 );  
  			target.questEndEffect;  
  			self.say( "그럼 잠시만 기다려. 네가 제뉴미스트 협회장의 시험을 통과하도록 만들어줄, 그 물건을 구해 놓을 테니." );  
  		}  
  	} else if ( ret == 2 ) {  
  		res = inven.exchange( 0, 4020002, -2 );  

  		if ( res == 0 ) self.say( "뭐야! #t4020002# 2개를  준비하지 못했잖아? 이봐. 거래를 하고 싶다면 준비를 제대로 해야지!" );  
  		else {  
  			qr.setState( 3301, 2 );  
  			target.questEndEffect;  
  			self.say( "그럼 잠시만 기다려. 네가 제뉴미스트 협회장의 시험을 통과하도록 만들어줄, 그 물건을 구해 놓을 테니." );  
  		}  
  	} else if ( ret == 3 ) {  
  		res = inven.exchange( 0, 4020003, -2 );  

  		if ( res == 0 ) self.say( "뭐야! #t4020003# 2개를  준비하지 못했잖아? 이봐. 거래를 하고 싶다면 준비를 제대로 해야지!" );  
  		else {  
  			qr.setState( 3301, 2 );  
  			target.questEndEffect;  
  			self.say( "그럼 잠시만 기다려. 네가 제뉴미스트 협회장의 시험을 통과하도록 만들어줄, 그 물건을 구해 놓을 테니." );  
  		}  
  	} else if ( ret == 4 ) {  
  		res = inven.exchange( 0, 4020004, -2 );  

  		if ( res == 0 ) self.say( "뭐야! #t4020004# 2개를  준비하지 못했잖아? 이봐. 거래를 하고 싶다면 준비를 제대로 해야지!" );  
  		else {  
  			qr.setState( 3301, 2 );  
  			target.questEndEffect;  
  			self.say( "그럼 잠시만 기다려. 네가 제뉴미스트 협회장의 시험을 통과하도록 만들어줄, 그 물건을 구해 놓을 테니." );  
  		}  
  	} else if ( ret == 5 ) {  
  		res = inven.exchange( 0, 4020005, -2 );  

  		if ( res == 0 ) self.say( "뭐야! #t4020005# 2개를  준비하지 못했잖아? 이봐. 거래를 하고 싶다면 준비를 제대로 해야지!" );  
  		else {  
  			qr.setState( 3301, 2 );  
  			target.questEndEffect;  
  			self.say( "그럼 잠시만 기다려. 네가 제뉴미스트 협회장의 시험을 통과하도록 만들어줄, 그 물건을 구해 놓을 테니." );  
  		}  
  	} else if ( ret == 6 ) {  
  		res = inven.exchange( 0, 4020006, -2 );  

  		if ( res == 0 ) self.say( "뭐야! #t4020006# 2개를  준비하지 못했잖아? 이봐. 거래를 하고 싶다면 준비를 제대로 해야지!" );  
  		else {  
  			qr.setState( 3301, 2 );  
  			target.questEndEffect;  
  			self.say( "그럼 잠시만 기다려. 네가 제뉴미스트 협회장의 시험을 통과하도록 만들어줄, 그 물건을 구해 놓을 테니." );  
  		}  
  	} else if ( ret == 7 ) {  
  		res = inven.exchange( 0, 4020007, -2 );  

  		if ( res == 0 ) self.say( "뭐야! #t4020007# 2개를  준비하지 못했잖아? 이봐. 거래를 하고 싶다면 준비를 제대로 해야지!" );  
  		else {  
  			qr.setState( 3301, 2 );  
  			target.questEndEffect;  
  			self.say( "그럼 잠시만 기다려. 네가 제뉴미스트 협회장의 시험을 통과하도록 만들어줄, 그 물건을 구해 놓을 테니." );  
  		}  
  	} else if ( ret == 8 ) {  
  	res = inven.exchange( 0, 4020008, -2 );  

  		if ( res == 0 ) self.say( "뭐야! #t4020008# 2개를  준비하지 못했잖아? 이봐. 거래를 하고 싶다면 준비를 제대로 해야지!" );  
  		else {  
  			qr.setState( 3301, 2 );  
  			target.questEndEffect;  
  			self.say( "그럼 잠시만 기다려. 네가 제뉴미스트 협회장의 시험을 통과하도록 만들어줄, 그 물건을 구해 놓을 테니." );  
  		}  
  	} else if ( ret == 9 ) {  
  		res = inven.exchange( 0, 4010004, -2 );  

  		if ( res == 0 ) self.say( "뭐야! #t4010004# 2개를  준비하지 못했잖아? 이봐. 거래를 하고 싶다면 준비를 제대로 해야지!" );  
  		else {  
  			qr.setState( 3301, 2 );  
  			target.questEndEffect;  
  			self.say( "그럼 잠시만 기다려. 네가 제뉴미스트 협회장의 시험을 통과하도록 만들어줄, 그 물건을 구해 놓을 테니." );  
  		}  
  	} else if ( ret == 10 ) {  
  		res = inven.exchange( 0, 4010006, -2 );  

  		if ( res == 0 ) self.say( "뭐야! #t4010006# 2개를  준비하지 못했잖아? 이봐. 거래를 하고 싶다면 준비를 제대로 해야지!" );  
  		else {  
  			qr.setState( 3301, 2 );  
  			target.questEndEffect;  
  			self.say( "그럼 잠시만 기다려. 네가 제뉴미스트 협회장의 시험을 통과하도록 만들어줄, 그 물건을 구해 놓을 테니." );  
  		}  
  	}  
  }  

  //알카드노 협회장의 시험  
  script "q3303e" {  
  	qr = target.questRecord;  
  	inven = target.inventory;  
	
  	j1 = 0; j2 = 0; j3 = 0; j4 = 0; j5 = 0; j6 = 0; j7 = 0; j8 = 0; j9 = 0; j10 = 0; j11 = 0;  

  	if ( inven.itemCount( 4020000 ) >= 2 ) j1 = 1;  
  	if ( inven.itemCount( 4020001 ) >= 2 ) j2 = 1;  
  	if ( inven.itemCount( 4020002 ) >= 2 ) j3 = 1;  
  	if ( inven.itemCount( 4020003 ) >= 2 ) j4 = 1;  
  	if ( inven.itemCount( 4020004 ) >= 2 ) j5 = 1;  
  	if ( inven.itemCount( 4020005 ) >= 2 ) j6 = 1;  
  	if ( inven.itemCount( 4020006 ) >= 2 ) j7 = 1;  
  	if ( inven.itemCount( 4020007 ) >= 2 ) j8 = 1;  
  	if ( inven.itemCount( 4020008 ) >= 2 ) j9 = 1;  
  	if ( inven.itemCount( 4010004 ) >= 2 ) j10 = 1;  
  	if ( inven.itemCount( 4010006 ) >= 2 ) j11 = 1;  

  	str = "";  
  	if ( j1 == 1 ) str = str + "\r\n#L0##b #t4020000##k#l";  
  	if ( j2 == 1 ) str = str + "\r\n#L1##b #t4020001##k#l";  
  	if ( j3 == 1 ) str = str + "\r\n#L2##b #t4020002##k#l";  
  	if ( j4 == 1 ) str = str + "\r\n#L3##b #t4020003##k#l";  
  	if ( j5 == 1 ) str = str + "\r\n#L4##b #t4020004##k#l";  
  	if ( j6 == 1 ) str = str + "\r\n#L5##b #t4020005##k#l";  
  	if ( j7 == 1 ) str = str + "\r\n#L6##b #t4020006##k#l";  
  	if ( j8 == 1 ) str = str + "\r\n#L7##b #t4020007##k#l";  
  	if ( j9 == 1 ) str = str + "\r\n#L8##b #t4020008##k#l";  
  	if ( j10 == 1 ) str = str + "\r\n#L9##b #t4010004##k#l";  
  	if ( j11 == 1 ) str = str + "\r\n#L10##b #t4010006##k#l";  

  	if ( str == "" ) {  
  		self.say( "뭐야, 댓가로 줄만한 원석을 가지고 있지 않잖아? 댓가가 없으면 거래도 없지." );  
  		end;  
  	}  

  	ret = self.askMenu( "오호... 표정을 보아하니 거래할 준비가 된 모양이군. 그렇게까지 해서 알카드노에 가입하고 싶다니... 이해할 수 없지만, 뭐 좋아. 댓가로 무엇을 주겠어?\r\n" + str );  
  	if ( ret == 0 ) {  
  		res = inven.exchange( 0, 4020000, -2 );  

  		if ( res == 0 ) self.say( "뭐야! #t4020000# 2개를  준비하지 못했잖아? 이봐. 거래를 하고 싶다면 준비를 제대로 해야지!" );  
  		else {  
  			qr.setState( 3303, 2 );  
  			target.questEndEffect;  
  			self.say( "그럼 잠시만 기다려. 네가 알카드노 협회장의 시험을 통과하도록 만들어줄, 그 물건을 구해 놓을 테니." );  
  		}  
  	} else if ( ret == 1 ) {  
  		res = inven.exchange( 0, 4020001, -2 );  

  		if ( res == 0 ) self.say( "뭐야! #t4020001# 2개를  준비하지 못했잖아? 이봐. 거래를 하고 싶다면 준비를 제대로 해야지!" );  
  		else {  
  			qr.setState( 3303, 2 );  
  			target.questEndEffect;  
  			self.say( "그럼 잠시만 기다려. 네가 알카드노 협회장의 시험을 통과하도록 만들어줄, 그 물건을 구해 놓을 테니." );  
  		}  
  	} else if ( ret == 2 ) {  
  		res = inven.exchange( 0, 4020002, -2 );  

  		if ( res == 0 ) self.say( "뭐야! #t4020002# 2개를  준비하지 못했잖아? 이봐. 거래를 하고 싶다면 준비를 제대로 해야지!" );  
  		else {  
  			qr.setState( 3303, 2 );  
  			target.questEndEffect;  
  			self.say( "그럼 잠시만 기다려. 네가 알카드노 협회장의 시험을 통과하도록 만들어줄, 그 물건을 구해 놓을 테니." );  
  		}  
  	} else if ( ret == 3 ) {  
  		res = inven.exchange( 0, 4020003, -2 );  

  		if ( res == 0 ) self.say( "뭐야! #t4020003# 2개를  준비하지 못했잖아? 이봐. 거래를 하고 싶다면 준비를 제대로 해야지!" );  
  		else {  
  			qr.setState( 3303, 2 );  
  			target.questEndEffect;  
  			self.say( "그럼 잠시만 기다려. 네가 알카드노 협회장의 시험을 통과하도록 만들어줄, 그 물건을 구해 놓을 테니." );  
  		}  
  	} else if ( ret == 4 ) {  
  		res = inven.exchange( 0, 4020004, -2 );  

  		if ( res == 0 ) self.say( "뭐야! #t4020004# 2개를  준비하지 못했잖아? 이봐. 거래를 하고 싶다면 준비를 제대로 해야지!" );  
  		else {  
  			qr.setState( 3303, 2 );  
  			target.questEndEffect;  
  			self.say( "그럼 잠시만 기다려. 네가 알카드노 협회장의 시험을 통과하도록 만들어줄, 그 물건을 구해 놓을 테니." );  
  		}  
  	} else if ( ret == 5 ) {  
  		res = inven.exchange( 0, 4020005, -2 );  

  		if ( res == 0 ) self.say( "뭐야! #t4020005# 2개를  준비하지 못했잖아? 이봐. 거래를 하고 싶다면 준비를 제대로 해야지!" );  
  		else {  
  			qr.setState( 3303, 2 );  
  			target.questEndEffect;  
  			self.say( "그럼 잠시만 기다려. 네가 알카드노 협회장의 시험을 통과하도록 만들어줄, 그 물건을 구해 놓을 테니." );  
  		}  
  	} else if ( ret == 6 ) {  
  		res = inven.exchange( 0, 4020006, -2 );  

  		if ( res == 0 ) self.say( "뭐야! #t4020006# 2개를  준비하지 못했잖아? 이봐. 거래를 하고 싶다면 준비를 제대로 해야지!" );  
  		else {  
  			qr.setState( 3303, 2 );  
  			target.questEndEffect;  
  			self.say( "그럼 잠시만 기다려. 네가 알카드노 협회장의 시험을 통과하도록 만들어줄, 그 물건을 구해 놓을 테니." );  
  		}  
  	} else if ( ret == 7 ) {  
  		res = inven.exchange( 0, 4020007, -2 );  

  		if ( res == 0 ) self.say( "뭐야! #t4020007# 2개를  준비하지 못했잖아? 이봐. 거래를 하고 싶다면 준비를 제대로 해야지!" );  
  		else {  
  			qr.setState( 3303, 2 );  
  			target.questEndEffect;  
  			self.say( "그럼 잠시만 기다려. 네가 알카드노 협회장의 시험을 통과하도록 만들어줄, 그 물건을 구해 놓을 테니." );  
  		}  
  	} else if ( ret == 8 ) {  
  	res = inven.exchange( 0, 4020008, -2 );  

  		if ( res == 0 ) self.say( "뭐야! #t4020008# 2개를  준비하지 못했잖아? 이봐. 거래를 하고 싶다면 준비를 제대로 해야지!" );  
  		else {  
  			qr.setState( 3303, 2 );  
  			target.questEndEffect;  
  			self.say( "그럼 잠시만 기다려. 네가 알카드노트 협회장의 시험을 통과하도록 만들어줄, 그 물건을 구해 놓을 테니." );  
  		}  
  	} else if ( ret == 9 ) {  
  		res = inven.exchange( 0, 4010004, -2 );  

  		if ( res == 0 ) self.say( "뭐야! #t4010004# 2개를  준비하지 못했잖아? 이봐. 거래를 하고 싶다면 준비를 제대로 해야지!" );  
  		else {  
  			qr.setState( 3303, 2 );  
  			target.questEndEffect;  
  			self.say( "그럼 잠시만 기다려. 네가 알카드노 협회장의 시험을 통과하도록 만들어줄, 그 물건을 구해 놓을 테니." );  
  		}  
  	} else if ( ret == 10 ) {  
  		res = inven.exchange( 0, 4010006, -2 );  

  		if ( res == 0 ) self.say( "뭐야! #t4010006# 2개를  준비하지 못했잖아? 이봐. 거래를 하고 싶다면 준비를 제대로 해야지!" );  
  		else {  
  			qr.setState( 3303, 2 );  
  			target.questEndEffect;  
  			self.say( "그럼 잠시만 기다려. 네가 알카드노 협회장의 시험을 통과하도록 만들어줄, 그 물건을 구해 놓을 테니." );  
  		}  
  	}  
  }  

  //화살지급  
  script "q6700e" {  
  	inven = target.inventory;  
  	qr = target.questRecord;  
  	file = "#fUI/UIWindow.img/QuestIcon/";  
  	if ( qr.getState( 6700 ) == 1 ) {  
  		ret = self.askMenu( "Eu preparei um presente para voc�. Isto ajudar� na sua viagem. Escolha as flechas que voc� deseja ter.\r\n" + file + "4/0\r\n\r\n#b#L0# #v2060000##t2060000# 6000 ea.#l\r\n#L1# #v2061000##t2061000# 6000 ea#l" );  
  		if ( ret == 0 ) {  
  			res = inven.exchange( 0, 2060000, 6000 );  
  			if ( res == 0 ) self.say( "Voc� precisa ter pelo menos 3 slots vazios em seu invent�rio de Uso." );  
  			else {  
  				qr.setState( 6700, 2 );  
  				target.questEndEffect;  
  				self.say( "Acabei de te dar 6000 itens do tipo #b#t2060000##k. Agora, melhor treinar para ser o melhor arqueiro do Mundo Maple." );  
  			}  
  		} else {  
  			res = inven.exchange( 0, 2061000, 6000 );  
  			if ( res == 0 ) self.say( "Voc� precisa ter pelo menos 3 slots vazios em seu invent�rio de Uso." );  
  			else {	  
  				qr.setState( 6700, 2 );  
  				target.questEndEffect;  
  				self.say( "Acabei de te dar 6000 itens do tipo #b#t2061000##k. Agora, melhor treinar para ser o melhor arqueiro do Mundo Maple." );		  
  			}  
  		}  
  	}  
  }  

  // 드랭이 바라는 것  
  script "q3353s" {  
  	qr = target.questRecord;  
  	self.say( "호오~ 친절한 모험가 친구! 왔는가? 오랜만이지? 자네가 정말정말 보고 싶었다네! 왜냐고? 후후후후... 전에 자네가 물어봤던 것에 대해 알아냈거든! 그, 왜 있잖은가. 그 성격 어두운 연금술사의 사념 말이야." );  
  	v0 = self.askAccept( "그 사람의 또 다른 사념의 흔적을 알아냈거든. 자네가 관심이 있는 것 같아서 열심히 찾아봤지. 후후후... 자, 그럼 어서 그에게 그에게 가보게" );  
  	if ( v0 == 0 ) self.say( "엥? 싫은가? 자네가 싫다면 하는 수 없지만... 자네가 관심 있어 하는 것 같아서 일부러 고생고생해서 알아놨는데 파웬의 호의를 이리도 무시하다니... 훌쩍훌쩍." );  
  	else {  
  		qr.setState( 3353, 1 );  
  		registerTransferField( 926120200, "out00" );  
  	}  
  }  

  // 드랭의 약  
  script "q3354s" {  
  	qr = target.questRecord;  
  	self.say( "휴우... 더 이상 연구에 진척이 없습니다. 사실상 실험은 실패한 것이나 다름 없지요. 아무리 연구해도 원래의 기억을 다 갖춘 채로 인간의 육체를 기계로 바꾸는 것은 불가능하단 걸 알게 되었거든요... 하지만... 대신 더 좋은 걸 만들었답니다. " );  
  	self.say( "그건 다름 아닌... 딸인 키니를 위한 약이지요. 키니는 선천적으로 몸이 약하답니다. 그저 원래 그런 것이라 생각했는데... 사실 그건 요정과 인간의 혼혈이기에 어쩔 수 없는 일이라더군요. 그래서 그 애를 위해 약을 개발했습니다. " );  
  	v0 = self.askAccept( "후후.. 정말 뿌듯하군요. 인간을 기계로 만들어 수명을 늘리는 연구는 실패해 버렸지만... 요정처럼 영원히 살지는 못하더라도 그 이상의 행복을 찾을 수 있으리란 생각이 듭니다... 아, 이만 연구를 마무리해야겠군요. 폭발물이 많아 위험하니 당신을 이 연구실에서 추방하겠습니다" );  
  	if ( v0 == 0 ) self.say( "아직 실험이 완전히 끝난 것은 아닙니다. 위험한 실험 도구가 많으니 자리를 피해 주십시오." );  
  	else {  
  		qr.setState( 3354, 1 );  
  		registerTransferField( 261020401, "" );  
  	}  
  }  

  //비밀번호 인증  
  script "q3360s" {  
  	qr = target.questRecord;  
	
  	self.say( "오! 자네 왔는가? 마침 잘 왔네. 자네를 위해 이 파웬이 비밀통로를 출입할 수 있게 해줄 마스터키를 알아냈다네! 하하하하! 굉장하지 않은가? 어서 굉장하다고 말하게!" );  
  	v1 = self.askAccept( "자아. 키가 굉장히 길고 복잡하니 잘 기억해 두길 바라겠네. 한 번만 말할 테니 어딘가에 적어 두라고. 준비 되었나?" );  

  	if ( v1 == 0 ) self.say( "빨리 빨리. 외울자신이 없으면 펜이라도 꺼내라고!" );  
  	else {  
  		str = shuffle( 1, "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ" );  
  		pass2 = substring( str, 0, 10 );  
  		qr.set( 7061, pass2 );  
  		qr.set( 7062, "00" );  
  		qr.set( 3360, "0" );  
  		qr.setState( 3360, 1 );  
  		self.say( "키번호는 #b" + pass2 + "#k이네. 잊지 않았겠지? 이 키를 비밀통로 입구에 입력하면 비밀통로를 자유롭게 이용할 수 있을 거야. " );  
  	}  
  }  
  /*  
  //스텀피의 묘목 기르기  
  script "q2147s" {  
  	inven = target.inventory;  
  	qr = target.questRecord;  

  	self.say( "다시 만나게 되는군. 오면서 #m102000000#의 상태를 보았나? 지난 번 자네의활약으로 #o3220000#를 쓰러트리고 나서 모든 것이 해결됐다고 생각했지만, 그건 나의 착각이었네. #m102000000#이 입은 상처는 생각보다 훨씬 컸어." );  
  	self.say( "이것이 무엇인지 알겠나? 바로 자네가 잡은 #o3220000#의 고목에서 돋아난 어린 묘목이라네. 자네가 왜 걱정스러운 표정을 짓는지는 알겠지만 염려말게. 이 어린 묘목들을 제대로 키운다면 #o3220000#로 자라나는 일은 없을거야." );  
  	v1 = self.askAccept( "내가 생각하기에는 자네가 이일의 적임자 같은데 자네 생각은 어떤가?" );  
	
  	if ( v1 == 0 ) self.say( "자네말고는 딱히 적임자가 생각나지 않는데... 자네가 사양한다니 이거 참 난감하군." );  
  	else {  
  		a1 = inven.exchange( 0, 4220020, 1 );  

  		if ( a1 == 0 ) self.say( "자네 짐이 너무 많아서 묘목을 건내 줄 수가 없을 것 같군. 기타 인벤토리를 한 칸이상 비우고 다시 오게." );  
  		else {  
  			qr.setState( 2147, 1 );  
  			self.say( "이 어린 묘목을 #m102000000#의 메마른 땅에 옮겨심을 수 있을 정도로만 키워주게. 지금은 너무 어려서 심는다고 해도 금방 죽어버리고 말거야. 어린 묘목을 잘 키워서 #m102000000#에 심는 일이 황폐해지고 있는 #m102000000#을 위해서 옮은 일이라고 난 믿네." );  
  		}  
  	}  
  }  

  script "q2147e" {  
  	inven = target.inventory;  
  	qr = target.questRecord;  
  	exp = qr.get( 7603 );  
  	lv = exp/1000;  


  	if ( itemCount( 4220020 ) <= 0 ) {  
  		self.say( "묘목을 잃어버린것 같군. 묘목은 작고 어리기 때문에 신경써서 돌봐야 한다는걸 잊지 말게. 내가 새로운 묘목을 한 개 더 주도록 하지." );  
  		a1 = inven.exchange( 0, 4220020, 1 );  

  		if ( a1 == 0 ) self.say( "자네 짐이 너무 많아서 묘목을 건내 줄 수가 없을 것 같군. 기타 인벤토리를 한 칸이상 비우고 다시 오게." );  
  	} else {  
  		if ( lv < 0 ) {  
  	}  
  }  
  */  

  //소문의 진상_돼지와 함께 춤을  
  script "q2148s" {  
  	qr = target.questRecord;  

  	am1 = self.askMenu( "무슨 일이지?\r\n#b#L0#귀신나무에 대해 들어보신 적이 있나요?#l" );  
	
  	if ( am1 == 0 ) {  
  		am2 = self.askMenu( "귀신나무? 아, 오래 전에 사라졌던 그 거대한 스텀프를 말하는 건가? 아버지의 아버지가 어릴 적에 그런 나무가 있었다는 이야기를 들은 적이 있었다네. 전해져 오는 소문에는 가지마다 붉은 천이 달려있는데 혼령의 피로 물들은거라고 하더군. 하지만 나도 실제로 본 적은 한 번도 없다네. 그러니 진실인지는 알 수 없지.\r\n#b#L0#다른 소문은 듣지 못했나요?#l" );  

  		if ( am2 == 0 ) {  
  			self.say( "애석하게도 나는 소문에 밝은 사람이 아니라네." );  
  			target.incEXP( 100, 0 );  
  			qr.setState( 2148, 1 );  
  			qr.setState( 2148, 2 );  
  			target.questEndEffect;  
  		}  
  	}  
  }  

  //소문의 진상_만지  
  script "q2149s" {  
  	qr = target.questRecord;  

  	am1 = self.askMenu( "...무슨 일이지?\r\n#b#L0#귀신나무에 대해 들어보신 적이 있나요?#l" );  

  	if ( am1 == 0 ) {  
  		am2 = self.askMenu( "겁쟁이들의 이야기를 들은 모양이군. 귀신나무라니 그런게 있을 리가 없지. 오랫동안 페리온의 바위산을 돌아다니며 수련을 했지만, 그런 나무는 본 적도 들은 적도 없어.\r\n#b#L0#아 그런가요?...#l" );  

  		if ( am2 == 0 ) {  
  			self.say( "단지 요즘 동쪽 바위산에서 의문의 습격을 받는 일이 늘어났다고 하는데, 조금 신경이 쓰이는군..." );  
  			target.incEXP( 100, 0 );  
  			qr.setState( 2149, 1 );  
  			qr.setState( 2149, 2 );  
  			target.questEndEffect;  
  		}  
  	}  
  }  

  //소문의 진상_이얀  
  script "q2150s" {  
  	qr = target.questRecord;  

  	am1 = self.askMenu( "안녕하세요 여행자님 오늘은 무슨 일로 오셨나요?\r\n#b#L0#귀신나무에 대해 알고 있니?#l" );  

  	if ( am1 == 0 ) {  
  		am2 = self.askMenu( "어머! 그 소문을 들으신거에요? 얼마 전에 헤네시스의 카밀라가 엄마 심부름으로 페리온에 왔다가 돌아가는 길에 귀신을 봤대요.\r\n#b#L0#정말이니?#l" );  

  		if ( am2 == 0 ) {  
  			self.say( "밤 늦게 헤네시스로 돌아가는 길이었는데 어둠속에서 나무줄기를 밟은 것 같아서 주위를 둘러보는데 희번덕거리는 눈이 카밀라를 잡아먹을 것처럼 쳐다봤다고 하더라구요." );  
  			self.say( "카밀라는 너무 무서워서 그대로 기절하고 말았대요. 날이 밝은 뒤에 어른들이 그 자리에 다시 가봤는데 아무 것도 없었대요. 귀신이 분명한 것 같아요. 어쩌죠? 이제 무서워서 마을 밖에 나갈 수가 없을 것 같아요." );  
  			target.incEXP( 100, 0 );  
  			qr.setState( 2150, 1 );  
  			qr.setState( 2150, 2 );  
  			target.questEndEffect;  
  		}  
  	}  
  }  

  //소문의 진상_주먹 펴고 일어서  
  script "q2151s" {  
  	qr = target.questRecord;  

  	am1 = self.askMenu( "무슨 일로 나를 찾아온건가?\r\n#b#L0#귀신나무에 대해 아시는 것이 있나요?#l" );  

  	if ( am1 == 0 ) {  
  		am2 = self.askMenu( "귀신나무라... 아마도 스텀피를 말하는 것 같군.\r\n#b#L0#스텀피가 뭔가요?#l" );  

  		if ( am2 == 0 ) {  
  			am3 = self.askMenu( "페리온이 아직 푸른 숲이었을때부터 지금까지 살아남은 아주 오래된 나무지. 하지만 오랜 세월을 지나는 동안 나무는 분노하기 시작했지. 숲을 파괴하는 인간을 보면서 분노했고, 메말라가는 숲을 보면서 분노했지\r\n#b#L0#그래서 어떻게 되었나요?#l" );  

  			if ( am3 == 0 ) {  
  				self.say( "결국 나무의 분노는 나무를 몬스터로 바꾸어 놓고 말았고, 이제는 닥치는 대로 땅의 양분을 갉아먹는 한낱 괴물이 되어버렸지. 너무 깊이 알려고 하지 말게. 자네의 호기심은 이해하지만, 그는 모든 스텀프들의 왕이야. 결코 쉽게 생각하면 안된다네." );  
  				target.incEXP( 100, 0 );  
  				qr.setState( 2151, 1 );  
  				qr.setState( 2151, 2 );  
  				target.questEndEffect;  
  			}  
  		}  
  	}  
  }  

  //소문의 진상_베티  
  script "q2152s" {  
  	qr = target.questRecord;  

  	am1 = self.askMenu( "어서와요. 용건이 뭐죠?\r\n#b#L0#귀신나무에 대해 아시는 것이 있나요?#l" );  

  	if ( am1 == 0 ) {  
  		am2 = self.askMenu( "윈스턴 박사님의 연구를 도와 주고 있나 보군요? 글쎄요. 저도 박사님의 부탁을 받고 조사를 좀 해봤는데 알아낸 것이 없어요. 단지 요즘 페리온과의 접경지대에 있는 엘리니아의 숲이 급속도로 메말라가기 시작했다는 것을 알아냈죠. 진행속도는 느리지만 경계해야 할 일이에요.\r\n#b#L0#네. 시간을 내주셔서 감사합니다.#l" );  

  		if ( am2 == 0 ) {  
  			self.say( "많은 도움이 되지 못한 것 같아서 미안하군요." );  
  			target.incEXP( 200, 0 );  
  			qr.setState( 2152, 1 );  
  			qr.setState( 2152, 2 );  
  			target.questEndEffect;  
  		}  
  	}  
  }  

  //스노우맨의 분노-단서 발견  
  script "q3108s" {  
  	qr = target.questRecord;  

  	self.say( "(조각상은 한 눈에 봐도 눈이 부실정도로 아름답다. 얼음으로 만들어진 것 같이 투명하지만 얼음은 아닌 것 같다. 조각상 주위를 돌며 자세히 살펴보았다.)" );  
  	self.say( "(조각상의 한쪽이 부셔져 있다. 주위에는 커다란 발자국도 몇 개 보인다." );  
  	target.incEXP( 200, 0 );  
  	qr.setState( 3108, 1 );  
  	qr.setState( 3108, 2 );  
  	target.questEndEffect;  
  }  

  //무지개색 달팽이  
  script "q2156e" {  
  	qr = target.questRecord;  
  	inven = target.inventory;  
  	file = "#fUI/UIWindow.img/QuestIcon/";  

  	if ( inven.itemCount( 2210006 ) >= 1 ) {  
  		self.say( "좋아. 이게 바로 무지개색 달팽이 껍질이라 이거지? \r\n\r\n" + file + "4/0#\r\n\r\n\r\n" + file + "8/0# 7500 exp" );  
  		ret = inven.exchange( 30000, 2210006, -1 );  
		
  		if ( ret == 0 ) self.say( "기타창과 소비창에 빈 칸이 있는지 확인하세요." );  
  		else {  
  			self.setSpecialAction( "act2156" );  
  			target.incEXP( 7500, 0 );  
  			pop = target.incPOP( 3, 0 );  
  			qr.setState( 2156, 2 );  
  			target.questEndEffect;  
  			self.say( "무지개색 달팽이 껍질을 나누기로 하지 않았냐고? 하지만 이걸 반으로 갈랐다가는 효능이 없어질지도 모르잖아? 먼저 정보를 준 건 이 쪽이니까, 당연히 내가 가져야지! 후훗!" );  
  		}  
  	} else {  
  		morphID = target.getMorphState;  

  		if ( morphID == 7 ){  
  			self.say( "뭐, 뭐야 그 모습은? 왜 달팽이가 되어서 온 거야?! 뭐? 무지개색 달팽이 껍질을 쓰니까 이렇게 되었다고? 그러고 보니 달팽이 전설에서도 보물의 위험에 대해 경고하는 말이 있었지.. 휴우. 다행이다. 안 먹어서. 욕심을 부리니까 그런 꼴이 된 거라고." );  
  			target.incEXP( 10000, 0 );  
  			pop = target.incPOP( -1, 0 );  
  			qr.setState( 2156, 2 );  
  			target.questEndEffect;  
  		}  
  		else self.say( "흠... #b#t2210006##k은 아직 구하지 못한 거야? 해안가 풀숲으로 가서 #b#o2220000##k를 잡아 보라니까?" );  
  	}  
  }  
