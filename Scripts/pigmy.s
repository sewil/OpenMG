module "standard.s";


//GM
function pigmy_master {
	if ( target.nJob == 900 ) {
		ret = self.askMenu( "How can i help you? \r\n#b#L0# Initializing recieved today data #l\r\n#L1# Initializing  recieved recently dates #l\r\n#L2# Going next step#l" );
		if ( ret == 0 ) {
			qr = target.questRecord;
			qr.setComplete( 9461 );
		} else if ( ret == 1 ) {
			qr = target.questRecord;
			qr.setComplete( 9460 );
		}
	}

	return;
}

//만약을 대비한 처리
function pigmy_reinit {
	qr = target.questRecord;
	num = length( qr.get( 9461 ));
	if ( num == 8 ) {
		result = qr.get( 9461 ) + "0";
		qr.set( 9461, result );
	}

	return;
}


//알 교환
function( integer ) egg_Code( integer tCode ) {
	if ( tCode == 0 ) return 4170000;
	if ( tCode == 1 ) return 4170001;
	if ( tCode == 2 ) return 4170002;
	if ( tCode == 3 ) return 4170003;
	if ( tCode == 4 ) return 4170004;
//	if ( tCode == 5 ) return 4170005;	blocked by JK 2008.08.18
//	if ( tCode == 6 ) return 4170006;	blocked by JK 2008.08.18
	return 4170000;
//	if ( tCode == 9 ) return 4170009;	blocked by JK 2008.08.13
//	return 4170007;			blocked by JK 2008.08.13
}

function( string ) pigmytownName( integer tCode ) {
	if ( tCode == 4170000 ) return "Henesys";	//henesys
	if ( tCode == 4170001 ) return "Ellinia";	//ellinia
	if ( tCode == 4170002 ) return "Cidade de Kerning";	//cerning city
	if ( tCode == 4170003 ) return "Perion";		// perion
	if ( tCode == 4170004 ) return "El Nath";		//el nath
//	if ( tCode == 4170005 ) return "Ludibrium";		blocked by JK 2008.08.18
//	if ( tCode == 4170006 ) return "Orbis";			blocked by JK 2008.08.18
	return "Henesys";
//	if ( tCode == 4170009 ) return "노틸러스";	              blocked by JK 2008.08.13
//	return "아쿠아리움";				blocked by JK 2008.08.13
}

function (integer) pigmytown_Code( integer mapCode ){
	if (mapCode == 100000000) code = 0;
	else if (mapCode == 101000000 ) code = 1;
	else if (mapCode == 103000000 ) code = 2;
	else if (mapCode == 102000000 ) code = 3;
	else if (mapCode == 211000000 ) code = 4;
//	else if (mapCode == 220000000 ) code = 5;	blocked by JK 2008.08.18
//	else if (mapCode == 200000000 ) code = 6;	blocked by JK 2008.08.18
//	else if (mapCode == 230000000 ) code = 7;	아쿠아리움 blocked by JK 2008.08.13
//	else if (mapCode == 120000000 ) code = 9;	노틸러스 blocked by JK 2008.08.13
	else code = 0;	
	
	return code;
}

function( integer ) eggChg( integer eggCode ) {
	inventory = target.inventory;
	eggNum = inventory.itemCount( eggCode );
	tName = pigmytownName( eggCode );

	if ( eggNum > 0 ) {
//		v1 = self.askMenu( "#b" + tName + " 피그미 에그#k를 어느 마을 피그미 에그로 교환하시겠습니까?\r\n#b#L0# 헤네시스#l\r\n#L1# 엘리니아#l\r\n#L2# 커닝시티#l\r\n#L3# 페리온#l\r\n#L4# 엘나스#l\r\n#L5# 루디브리엄#l\r\n#L6# 오르비스#l\r\n#L7# 아쿠아리움#l\r\n#L9# 노틸러스#l  " );		// modified by JK 2008.08.13
		v1 = self.askMenu( "#b" + tName + " : #kVoc� gostaria de trocar seu Ovo de Pigmy com o de qual cidade?\r\n#b#L0# Henesys#l\r\n#L1# Ellinia#l\r\n#L2# Cidade de Kerning#l\r\n#L3# Perion#l\r\n#L4# El Nath#l  " );
//		v1 = self.askMenu( "#b" + tName + " 피그미 에그#k를 어느 마을 피그미 에그로 교환하시겠습니까?\r\n#b#L5# 루디브리엄#l\r\n#L7# 아쿠아리움#l\r\n#L9# 노틸러스#l  " );
		chgedNum = egg_Code( v1 );
		if ( chgedNum == eggCode ) {
			self.say( "Voc� n�o pode trocar ovos da mesma cidade." );
			end;
		}
//		eggchgNum = self.askNumber( "#b" + target.sCharacterName + "님#k은 " + tName + " 피그미 에그를 #b" + eggNum + "개#k 가지고 있습니다.  몇 개를 교환하시 겠습니까?\r\n#b< 예 : 3 >#k", 0, 0, eggNum );
//		eggchgNum = self.askNumber( "#b" + target.sCharacterName + "님#k은 " + tName + " 피그미 에그를 #b" + eggNum + "개#k 가지고 있습니다.  Quantos voce gostaria de trocar?\r\n#b< 예 : 3 >#k", 0, 0, eggNum );
		eggchgNum = self.askNumber( "#b" + target.sCharacterName + " #kpossui#b " + eggNum + " #kOvo(s) de Pigmy de#b " + tName + ".Quantos voc� gostaria de trocar?\r\n#b< ex : 3 >#k", 0, 0, eggNum );

		if ( eggchgNum == 0 ) {
			self.say( "Voc� n�o pode trocar 0." );
			return 0;
		}
		ret = inventory.exchange( 0, eggCode, -eggchgNum, chgedNum, eggchgNum );
		if ( ret == 0 ) return -1;
		return 0;
	} else {
//		self.say( "#b" + target.sCharacterName + "님#k은 " + tName + " 피그미 에그를 가지고 있지 않습니다." );
		self.say( "#b" + target.sCharacterName + " #kn�o possui Ovos de Pigmy de " + tName + "." );

		return 0;
	}
	return -2;
}

function pigmy_exchange {
//	v0 = self.askMenu( "피그미가 자신이 낳은 알을 다른 지역의 알과 바꾸어 주겠다고 합니다. 어느 마을 피그미 에그를 교환하시겠습니까?\r\n#b#L0# 헤네시스#l\r\n#b#L1# 엘리니아#l\r\n#b#L2# 커닝시티#l\r\n#b#L3# 페리온#l\r\n#b#L4# 엘나스#l\r\n#b#L5# 루디브리엄#l\r\n#b#L6# 오르비스#l\r\n#b#L7# 아쿠아리움#l\r\n#b#L9# 노틸러스#l " );	// modified by JK 2008.08.13
	v0 = self.askMenu( "O Pigmy permite que seus ovos sejam trocados por ovos de outra cidade. Voc� gostaria de trocar seu Ovo de Pigmy com o de qual cidade?\r\n#b#L0# Henesys#l\r\n#b#L1# Ellinia#l\r\n#b#L2# Cidade de Kerning#l\r\n#b#L3# Perion#l\r\n#b#L4# El Nath#l " );
//	v0 = self.askMenu( "피그미가 자신이 낳은 알을 다른 지역의 알과 바꾸어 주겠다고 합니다. 어느 마을 피그미 에그를 교환하시겠습니까?\r\n#b#L5# 루디브리엄#l\r\n#b#L7# 아쿠아리움#l\r\n#b#L9# 노틸러스#l " );
	if ( v0 == 0 ) {
		ret = eggChg( 4170000 );
	} else if ( v0 == 1 ) {
		ret = eggChg( 4170001 );
	} else if ( v0 == 2 ) {
		ret = eggChg( 4170002 );
	} else if ( v0 == 3 ) {
		ret = eggChg( 4170003 );
	} else if ( v0 == 4 ) {
		ret = eggChg( 4170004 );
	} 

	/* blocked by JK 2008.08.13
	   else if ( v0 == 5 ) {
		ret = eggChg( 4170005 );
	} else if ( v0 == 6 ) {
		ret = eggChg( 4170006 );
	} 
	  else if ( v0 == 7 ) {
		ret = eggChg( 4170007 );
	} else if ( v0 == 9 ) {
		ret = eggChg( 4170009 );
	} 
	*/ 
	  else {
		self.say( "Um erro ocorreu. Por favor tente novamente mais tarde." );
		return;
	}

	if ( ret == -1 ) self.say( "Por favor, verifique se seu inventario possui slots vazios." );
	if ( ret == -2 ) self.say( "Um erro ocorreu. Por favor tente novamente mais tarde." );
	return;
}

script "pigmy"{
/*	field = self.field;
	id = field.id;
	code = pigmytown_Code(id);
	
	v0 = self.askMenu( "O que voc� gostaria de fazer com o Pigmy? \r\n#b#L0# Dar Ra豫o..#l\r\n#L1# Parece que o Pigmy quer dizer algo.#l" );	
	if ( v0 == 0 ){
		inventory = target.inventory;
		if ( inventory.itemCount( 2120008 ) >= 1 ) {
			ret1 = self.askYesNo( "O Pigmy parece com fome. Voce gostaria de dar #bRa豫o #kpara o Pigmy?" );
			if ( ret1 != 0 ) {
				nSlot = inventory.slotCount( 4 );
				nHold = inventory.holdCount( 4 );
				if ( nSlot > nHold or ( nSlot == nHold and inventory.itemCount( 4170000 + code ) % 100 > 0 ) ) {//빈 슬롯이 있는지 체크 or 빈 슬롯이 없어도 아이템이 추가될 수 있는지 체크
					ret2 = inventory.makePigmyEgg( 2120008, code );
					if ( ret2 >= 1 ) {
						self.say( "A Ra豫o. melhorou o humor do Pigmy! Ele botou um ovo." );
					} else {
						self.say( "O Pigmy gostou da comida, mas n�o botou um ovo." );
					}
				} else self.say( "Por favor, verifique se seu slot etc. est� vazio." );
			} else self.say( "O Pigmy parece famindo... Por favor d� uma ra豫o na pr�xima vez." );
		} else self.say( "O Pigmy � um gourmet. S� alimente-o com a #bRa豫o." );
	}
	else {
		pigmy_exchange;
	}
*/
	pigmy_reinit;
	
	v0 = self.askMenu( "O que voc� gostaria de fazer com o Pigmy? \r\n#b#L0# Dar Ra豫o..#l\r\n#L1# Parece que o Pigmy quer dizer algo.#l" );
	if ( v0 == 0 ) {
		//time check
		qr = target.questRecord;
		con = qr.get( 9460); //시간
		con2 = qr.get( 9461); //마을별코드
		wTime = currentTime;

//		self.say(  "dd" + substring( wTime, 0, 8 ) + " " + con2 + " " + con );

		//퀘스트 초기화
		if( con == "" or con2 == "" ) {
			qr.set( 9460, substring( wTime, 0, 8 ) );
			qr.set( 9461, "000000000" );
			con = qr.get( 9460); //시간
			con2 = qr.get( 9461); //마을별코드
		} else if ( con != substring( wTime, 0, 8)) {
			qr.set( 9460, substring( wTime, 0, 8));
			qr.set( 9461, "000000000" );
			con = qr.get( 9460); //시간
			con2 = qr.get( 9461); //마을별코드
		}

		if ( con != "" ) {
			if (  con == substring( wTime, 0, 8 )) {
				if ( substring( con2, 0,1 ) != "5" ) {
					inventory = target.inventory;
					if ( inventory.itemCount( 2120008 ) >= 1 ) {		// 맛있는 사료 ID
						ret1 = self.askYesNo( "O Pigmy parece com fome. Voce gostaria de dar #bRa豫o #kpara o Pigmy?" );
						if ( ret1 != 0 ) {
							nSlot = inventory.slotCount( 4 );
							nHold = inventory.holdCount( 4 );
							if ( nSlot > nHold or ( nSlot == nHold and inventory.itemCount( 4170000 ) % 100 > 0 ) ) {
								ret2 = inventory.makePigmyEgg( 2120008, 0 );
								if ( ret2 >= 1 ) {
									pigsubA = substring( con2, 0, 1 );
									pigorg = integer( pigsubA );
									pigsum = pigorg + 1;
									pigstr = string(pigsum);
									con3 = pigstr + substring( con2, 1, 8 );
									qr.set( 9461, con3 );
									self.say( "A Ra豫o. melhorou o humor do Pigmy! Ele botou um ovo." );
								} else {
									pigsubA = substring( con2, 0, 1 );
									pigorg = integer( pigsubA );
									pigsum = pigorg + 1;
									pigstr = string(pigsum);
									con3 = pigstr + substring( con2, 1, 8 );
									qr.set( 9461, con3 );	
									if ( serverType == 2 ) {
										if ( target.nJob == 900 ) target.message( con3 );
									}									
									self.say( "O Pigmy gostou da comida, mas n�o botou um ovo." );
								}
							} else self.say( "Por favor, verifique se seu slot etc. est� vazio." );
						} else self.say( "O Pigmy parece famindo... Por favor d� uma ra豫o na pr�xima vez." );
					} else self.say( "O Pigmy � um gourmet. S� alimente-o com a #bRa豫o." );
				} else {
					self.say( "O Pigmy est� satisfeito e n�o comer� mais." );
					end;
				}
			}
		}
	} else {
		pigmy_exchange;
		end;
	}
}



// Henesys
script "pigmy0" {
	pigmy_reinit;
	
	v0 = self.askMenu( "O que voc� gostaria de fazer com o Pigmy? \r\n#b#L0# Dar Ra豫o..#l\r\n#L1# Parece que o Pigmy quer dizer algo.#l" );
	if ( v0 == 0 ) {
		//time check
		qr = target.questRecord;
		con = qr.get( 9460); //시간
		con2 = qr.get( 9461); //마을별코드
		wTime = currentTime;

//		self.say(  "dd" + substring( wTime, 0, 8 ) + " " + con2 + " " + con );

		//퀘스트 초기화
		if( con == "" or con2 == "" ) {
			qr.set( 9460, substring( wTime, 0, 8 ) );
			qr.set( 9461, "000000000" );
			con = qr.get( 9460); //시간
			con2 = qr.get( 9461); //마을별코드
		} else if ( con != substring( wTime, 0, 8)) {
			qr.set( 9460, substring( wTime, 0, 8));
			qr.set( 9461, "000000000" );
			con = qr.get( 9460); //시간
			con2 = qr.get( 9461); //마을별코드
		}

		if ( con != "" ) {
			if (  con == substring( wTime, 0, 8 )) {
				if ( substring( con2, 0,1 ) != "1" ) {
					inventory = target.inventory;
					if ( inventory.itemCount( 2120008 ) >= 1 ) {		// 맛있는 사료 ID
						ret1 = self.askYesNo( "O Pigmy parece com fome. Voce gostaria de dar #bRa豫o #kpara o Pigmy?" );
						if ( ret1 != 0 ) {
							nSlot = inventory.slotCount( 4 );
							nHold = inventory.holdCount( 4 );
							if ( nSlot > nHold or ( nSlot == nHold and inventory.itemCount( 4170000 ) % 100 > 0 ) ) {
								ret2 = inventory.makePigmyEgg( 2120008, 0 );
								if ( ret2 >= 1 ) {
									con3 = "1" + substring( con2, 1, 8 );
									qr.set( 9461, con3 );
									self.say( "A Ra豫o. melhorou o humor do Pigmy! Ele botou um ovo." );
								} else {
									con3 = "1" + substring( con2, 1, 8 );
									qr.set( 9461, con3 );		
									if ( serverType == 2 ) {
										if ( target.nJob == 900 ) target.message( con3 );
									}									
									self.say( "O Pigmy gostou da comida, mas n�o botou um ovo." );
								}
							} else self.say( "Por favor, verifique se seu slot etc. est� vazio." );
						} else self.say( "O Pigmy parece famindo... Por favor d� uma ra豫o na pr�xima vez." );
					} else self.say( "O Pigmy � um gourmet. S� alimente-o com a #bRa豫o." );
				} else {
					self.say( "O Pigmy est� satisfeito e n�o comer� mais." );
					end;
				}
			}
		}
	} else {
		pigmy_exchange;
		end;
	}
}

//Ellinia
script "pigmy1" {
	pigmy_reinit;

	v0 = self.askMenu( "O que voc� gostaria de fazer com o Pigmy? \r\n#b#L0# Dar Ra豫o..#l\r\n#L1# Parece que o Pigmy quer dizer algo.#l" );
	if ( v0 == 0 ) {
		qr = target.questRecord;
		con = qr.get( 9460);
		con2 = qr.get( 9461);
		wTime = currentTime;

		//최초 초기화
		//퀘스트 초기화
		if( con == "" or con2 == "" ) {
			qr.set( 9460, substring( wTime, 0, 8 ) );
			qr.set( 9461, "000000000" );
			con = qr.get( 9460); //시간
			con2 = qr.get( 9461); //마을별코드
		} else if ( con != substring( wTime, 0, 8)) {
			qr.set( 9460, substring( wTime, 0, 8));
			qr.set( 9461, "000000000" );
			con = qr.get( 9460); //시간
			con2 = qr.get( 9461); //마을별코드
		}

//		self.say(  "dd" + substring( wTime, 0, 8 ) + " " + con2 + " " + con );

		if ( con != "" ) {
			if (  con == substring( wTime, 0, 8 )) {
				if ( substring( con2, 1,1 ) != "5" ) {
					inventory = target.inventory;
					if ( inventory.itemCount( 2120008 ) >= 1 ) {		// 맛있는 사료 ID
						ret1 = self.askYesNo( "O Pigmy parece com fome. Voce gostaria de dar #bRa豫o #kpara o Pigmy?" );
						if ( ret1 != 0 ) {
							nSlot = inventory.slotCount( 4 );
							nHold = inventory.holdCount( 4 );
							if ( nSlot > nHold or ( nSlot == nHold and inventory.itemCount( 4170001 ) % 100 > 0 ) ) {
								ret2 = inventory.makePigmyEgg( 2120008, 1 );
								if ( ret2 >= 1 ) {
									pigsubA = substring( con2, 1,1  );
									pigorg = integer( pigsubA );
									pigsum = pigorg + 1;
									pigstr = string(pigsum);
									con3 = substring( con2, 0, 1 ) + pigstr + substring( con2, 2, 7 );
									qr.set( 9461, con3 );
									self.say( "A Ra豫o. melhorou o humor do Pigmy! Ele botou um ovo." );
								} else {
									pigsubA = substring( con2, 1,1  );
									pigorg = integer( pigsubA );
									pigsum = pigorg + 1;
									pigstr = string(pigsum);
									con3 = substring( con2, 0, 1 ) + pigstr + substring( con2, 2, 7 );
									qr.set( 9461, con3 );	
									if ( serverType == 2 ) {
										if ( target.nJob == 900 ) target.message( con3 );
									}									
									self.say( "O Pigmy gostou da comida, mas n�o botou um ovo." );
								}			
							} else self.say( "Por favor, verifique se seu slot etc. est� vazio." );
						} else self.say( "O Pigmy parece famindo... Por favor d� uma ra豫o na pr�xima vez." );
					} else self.say( "O Pigmy � um gourmet. S� alimente-o com a #bRa豫o." );
				}  else {
					self.say( "O Pigmy est� satisfeito e n�o comer� mais." );
					end;
				}
			}
		}
	} else {
		pigmy_exchange;
		end;
	}
}

//Cerning City
script "pigmy2" {
	pigmy_reinit;

	v0 = self.askMenu( "O que voc� gostaria de fazer com o Pigmy? \r\n#b#L0# Dar Ra豫o..#l\r\n#L1# Parece que o Pigmy quer dizer algo.#l" );
	if ( v0 == 0 ) {
		qr = target.questRecord;
		con = qr.get( 9460);
		con2 = qr.get( 9461);
		wTime = currentTime;

	//	self.say(  "dd" + substring( wTime, 0, 8 ) + " " + con2 + " " + con );

		//퀘스트 초기화
		if( con == "" or con2 == "" ) {
			qr.set( 9460, substring( wTime, 0, 8 ) );
			qr.set( 9461, "000000000" );
			con = qr.get( 9460); //시간
			con2 = qr.get( 9461); //마을별코드
		} else if ( con != substring( wTime, 0, 8)) {
			qr.set( 9460, substring( wTime, 0, 8));
			qr.set( 9461, "000000000" );
			con = qr.get( 9460); //시간
			con2 = qr.get( 9461); //마을별코드
		}

		if ( con != "" ) {
			if (  con == substring( wTime, 0, 8 )) {
				if ( substring( con2, 2,1 ) != "5" ) {
					inventory = target.inventory;
					if ( inventory.itemCount( 2120008 ) >= 1 ) {		// 맛있는 사료 ID
						ret1 = self.askYesNo( "O Pigmy parece com fome. Voce gostaria de dar #bRa豫o #kpara o Pigmy?" );
						if ( ret1 != 0 ) {
							nSlot = inventory.slotCount( 4 );
							nHold = inventory.holdCount( 4 );
							if ( nSlot > nHold or ( nSlot == nHold and inventory.itemCount( 4170002 ) % 100 > 0 ) ) {
								ret2 = inventory.makePigmyEgg( 2120008, 2 );
								if ( ret2 >= 1 ) {
									pigsubA = substring( con2, 2, 1 );
									pigorg = integer( pigsubA );
									pigsum = pigorg + 1;
									pigstr = string(pigsum);
									con3 = substring( con2, 0, 2 ) + pigstr + substring( con2, 3, 6 );
									qr.set( 9461, con3 );
									self.say( "A Ra豫o. melhorou o humor do Pigmy! Ele botou um ovo." );
								}
								else {
									pigsubA = substring( con2, 2, 1 );
									pigorg = integer( pigsubA );
									pigsum = pigorg + 1;
									pigstr = string(pigsum);
									con3 = substring( con2, 0, 2 ) + pigstr + substring( con2, 3, 6 );
									qr.set( 9461, con3 );	
									if ( serverType == 2 ) {
										if ( target.nJob == 900 ) target.message( con3 );
									}									
									self.say( "O Pigmy gostou da comida, mas n�o botou um ovo." );
								}
							}
							else self.say( "Por favor, verifique se seu slot etc. est� vazio." );
						} 
						else self.say( "O Pigmy parece famindo... Por favor d� uma ra豫o na pr�xima vez." );
					}
					else self.say( "O Pigmy � um gourmet. S� alimente-o com a #bRa豫o." );
				} else {
					self.say( "O Pigmy est� satisfeito e n�o comer� mais." );
					end;
				}
			}
		}
	} else {
		pigmy_exchange;
		end;
	}
}

//Perion
script "pigmy3" {
	pigmy_reinit;

	v0 = self.askMenu( "O que voc� gostaria de fazer com o Pigmy? \r\n#b#L0# Dar Ra豫o..#l\r\n#L1# Parece que o Pigmy quer dizer algo.#l" );
	if ( v0 == 0 ) {
		qr = target.questRecord;
		con = qr.get( 9460);
		con2 = qr.get( 9461);
		wTime = currentTime;

//		self.say(  "dd" + substring( wTime, 0, 8 ) + " " + con2 + " " + con );

		//퀘스트 초기화
		if( con == "" or con2 == "" ) {
			qr.set( 9460, substring( wTime, 0, 8 ) );
			qr.set( 9461, "000000000" );
			con = qr.get( 9460); //시간
			con2 = qr.get( 9461); //마을별코드
		} else if ( con != substring( wTime, 0, 8)) {
			qr.set( 9460, substring( wTime, 0, 8));
			qr.set( 9461, "000000000" );
			con = qr.get( 9460); //시간
			con2 = qr.get( 9461); //마을별코드
		}

		if ( con != "" ) {
			if (  con == substring( wTime, 0, 8 )) {
				if ( substring( con2, 3,1 ) != "5" ) {
					inventory = target.inventory;
					if ( inventory.itemCount( 2120008 ) >= 1 ) {		// 맛있는 사료 ID
						ret1 = self.askYesNo( "O Pigmy parece com fome. Voce gostaria de dar #bRa豫o #kpara o Pigmy?" );
						if ( ret1 != 0 ) {
							nSlot = inventory.slotCount( 4 );
							nHold = inventory.holdCount( 4 );
							if ( nSlot > nHold or ( nSlot == nHold and inventory.itemCount( 4170003 ) % 100 > 0 ) ) {
								ret2 = inventory.makePigmyEgg( 2120008, 3 );
								if ( ret2 >= 1 ) {
									pigsubA = substring( con2, 3, 1 );
									pigorg = integer( pigsubA );
									pigsum = pigorg + 1;
									pigstr = string(pigsum);
									con3 = substring( con2, 0, 3 ) + pigstr + substring( con2, 4, 5 );
									qr.set( 9461, con3 );	
									self.say( "A Ra豫o. melhorou o humor do Pigmy! Ele botou um ovo." );
								}
								else {
									pigsubA = substring( con2, 3, 1 );
									pigorg = integer( pigsubA );
									pigsum = pigorg + 1;
									pigstr = string(pigsum);
									con3 = substring( con2, 0, 3 ) + pigstr + substring( con2, 4, 5 );
									qr.set( 9461, con3 );	
									if ( serverType == 2 ) {
										if ( target.nJob == 900 ) target.message( con3 );
									}									
									self.say( "O Pigmy gostou da comida, mas n�o botou um ovo." );
								}
							}
							else self.say( "Por favor, verifique se seu slot etc. est� vazio." );
						} 
						else self.say( "O Pigmy parece famindo... Por favor d� uma ra豫o na pr�xima vez." );
					}
					else self.say( "O Pigmy � um gourmet. S� alimente-o com a #bRa豫o." );
				} else {
					self.say( "O Pigmy est� satisfeito e n�o comer� mais." );
					end;
				}
			}
		}
	} else {
		pigmy_exchange;
		end;
	}
}

//El Nath
script "pigmy4" {
	pigmy_reinit;

	v0 = self.askMenu( "O que voc� gostaria de fazer com o Pigmy? \r\n#b#L0# Dar Ra豫o..#l\r\n#L1# Parece que o Pigmy quer dizer algo.#l" );
	if ( v0 == 0 ) {
		qr = target.questRecord;
		con = qr.get( 9460);
		con2 = qr.get( 9461);
		wTime = currentTime;

	//	self.say(  "dd" + substring( wTime, 0, 8 ) + " " + con2 + " " + con );

		//퀘스트 초기화
		if( con == "" or con2 == "" ) {
			qr.set( 9460, substring( wTime, 0, 8 ) );
			qr.set( 9461, "000000000" );
			con = qr.get( 9460); //시간
			con2 = qr.get( 9461); //마을별코드
		} else if ( con != substring( wTime, 0, 8)) {
			qr.set( 9460, substring( wTime, 0, 8));
			qr.set( 9461, "000000000" );
			con = qr.get( 9460); //시간
			con2 = qr.get( 9461); //마을별코드
		}

		if ( con != "" ) {
			if (  con == substring( wTime, 0, 8 )) {
				if ( substring( con2, 4,1 ) != "5" ) {
					inventory = target.inventory;
					if ( inventory.itemCount( 2120008 ) >= 1 ) {		// 맛있는 사료 ID
						ret1 = self.askYesNo( "O Pigmy parece com fome. Voce gostaria de dar #bRa豫o #kpara o Pigmy?" );
						if ( ret1 != 0 ) {
							nSlot = inventory.slotCount( 4 );
							nHold = inventory.holdCount( 4 );
							if ( nSlot > nHold or ( nSlot == nHold and inventory.itemCount( 4170004 ) % 100 > 0 ) ) {
								ret2 = inventory.makePigmyEgg( 2120008, 4 );
								if ( ret2 >= 1 ) {
									pigsubA = substring( con2, 4, 1 );
									pigorg = integer( pigsubA );
									pigsum = pigorg + 1;
									pigstr = string(pigsum);
									con3 = substring( con2, 0, 4 ) + pigstr + substring( con2, 5, 4 );
									qr.set( 9461, con3 );
									self.say( "A Ra豫o. melhorou o humor do Pigmy! Ele botou um ovo." );
								}
								else {
									pigsubA = substring( con2, 4, 1 );
									pigorg = integer( pigsubA );
									pigsum = pigorg + 1;
									pigstr = string(pigsum);
									con3 = substring( con2, 0, 4 ) + pigstr + substring( con2, 5, 4 );
									qr.set( 9461, con3 );
									if ( serverType == 2 ) {
										if ( target.nJob == 900 ) target.message( con3 );
									}
									self.say( "O Pigmy gostou da comida, mas n�o botou um ovo." );
								}
							}
							else self.say( "Por favor, verifique se seu slot etc. est� vazio." );
						} 
						else self.say( "O Pigmy parece famindo... Por favor d� uma ra豫o na pr�xima vez." );
					}
					else self.say( "O Pigmy � um gourmet. S� alimente-o com a #bRa豫o." );
				} else {
					self.say( "O Pigmy est� satisfeito e n�o comer� mais." );
					end;
				}
			}
		}
	} else {
		pigmy_exchange;
		end;
	}
}

/* blocked by JK 2008.08.18
//Ludiburium
script "pigmy5" {
	pigmy_reinit;

	v0 = self.askMenu( "O que voc� gostaria de fazer com o Pigmy? \r\n#b#L0# Dar Ra豫o..#l\r\n#L1# Parece que o Pigmy quer dizer algo.#l" );
	if ( v0 == 0 ) {
		qr = target.questRecord;
		con = qr.get( 9460);
		con2 = qr.get( 9461);
		wTime = currentTime;

	//	self.say(  "dd" + substring( wTime, 0, 8 ) + " " + con2 + " " + con );

		//퀘스트 초기화
		if( con == "" or con2 == "" ) {
			qr.set( 9460, substring( wTime, 0, 8 ) );
			qr.set( 9461, "000000000" );
			con = qr.get( 9460); //시간
			con2 = qr.get( 9461); //마을별코드
		} else if ( con != substring( wTime, 0, 8)) {
			qr.set( 9460, substring( wTime, 0, 8));
			qr.set( 9461, "000000000" );
			con = qr.get( 9460); //시간
			con2 = qr.get( 9461); //마을별코드
		}

		if ( con != "" ) {
			if (  con == substring( wTime, 0, 8 )) {
				if ( substring( con2, 5,1 ) != "1" ) {
					inventory = target.inventory;
					if ( inventory.itemCount( 2120008 ) >= 1 ) {		// 맛있는 사료 ID
						ret1 = self.askYesNo( "O Pigmy parece com fome. Voce gostaria de dar #bRa豫o #kpara o Pigmy?" );
						if ( ret1 != 0 ) {
							nSlot = inventory.slotCount( 4 );
							nHold = inventory.holdCount( 4 );
							if ( nSlot > nHold or ( nSlot == nHold and inventory.itemCount( 4170005 ) % 100 > 0 ) ) {
								ret2 = inventory.makePigmyEgg( 2120008, 5 );
								if ( ret2 >= 1 ) {
									con3 = substring( con2, 0, 5 ) + "1" + substring( con2, 6, 3 );
									qr.set( 9461, con3 );
									self.say( "A Ra豫o. melhorou o humor do Pigmy! Ele botou um ovo." );
								}
								else {
									con3 = substring( con2, 0, 5 ) + "1" + substring( con2, 6, 3 );
									qr.set( 9461, con3 );
									if ( serverType == 2 ) {
										if ( target.nJob == 900 ) target.message( con3 );
									}
									self.say( "O Pigmy gostou da comida, mas n�o botou um ovo." );
								}
							}
							else self.say( "Por favor, verifique se seu slot etc. est� vazio." );
						} 
						else self.say( "O Pigmy parece famindo... Por favor d� uma ra豫o na pr�xima vez." );
					}
					else self.say( "O Pigmy � um gourmet. S� alimente-o com a #bRa豫o." );
				} else {
					self.say( "O Pigmy est� satisfeito e n�o comer� mais." );
					end;
				}
			}
		}
	} else {
		pigmy_exchange;
		end;
	}
}
*/

/* blocked by JK 2008.08.13
//Orbis
script "pigmy6" {
	pigmy_reinit;

	//if ( serverType == 2 ) {
	//	self.say( "지금은 노틸러스, 루디브리엄, 아쿠아로드의 피그미로부터만 알을 얻을 수 있습니다." );
	//	end;
	//}

	v0 = self.askMenu( "O que voc� gostaria de fazer com o Pigmy? \r\n#b#L0# Dar Ra豫o..#l\r\n#L1# Parece que o Pigmy quer dizer algo.#l" );
	if ( v0 == 0 ) {
		qr = target.questRecord;
		con = qr.get( 9460);
		con2 = qr.get( 9461);
		wTime = currentTime;

	//	self.say(  "dd" + substring( wTime, 0, 8 ) + " " + con2 + " " + con );

		//퀘스트 초기화
		if( con == "" or con2 == "" ) {
			qr.set( 9460, substring( wTime, 0, 8 ) );
			qr.set( 9461, "000000000" );
			con = qr.get( 9460); //시간
			con2 = qr.get( 9461); //마을별코드
		} else if ( con != substring( wTime, 0, 8)) {
			qr.set( 9460, substring( wTime, 0, 8));
			qr.set( 9461, "000000000" );
			con = qr.get( 9460); //시간
			con2 = qr.get( 9461); //마을별코드
		}

		if ( con != "" ) {
			if (  con == substring( wTime, 0, 8 )) {
				if ( substring( con2, 6,1 ) != "1" ) {
					inventory = target.inventory;
					if ( inventory.itemCount( 2120008 ) >= 1 ) {		// 맛있는 사료 ID
						ret1 = self.askYesNo( "O Pigmy parece com fome. Voce gostaria de dar #bRa豫o #kpara o Pigmy?" );
						if ( ret1 != 0 ) {
							nSlot = inventory.slotCount( 4 );
							nHold = inventory.holdCount( 4 );
							if ( nSlot > nHold or ( nSlot == nHold and inventory.itemCount( 4170006 ) % 100 > 0 ) ) {
								ret2 = inventory.makePigmyEgg( 2120008, 6 );
								if ( ret2 >= 1 ) {
									con3 = substring( con2, 0, 6 ) + "1" + substring( con2, 7, 2 );
									qr.set( 9461, con3 );
									self.say( "A Ra豫o. melhorou o humor do Pigmy! Ele botou um ovo." );
								}
								else {
									con3 = substring( con2, 0, 6 ) + "1" + substring( con2, 7, 2 );
									qr.set( 9461, con3 );		
									if ( serverType == 2 ) {
										if ( target.nJob == 900 ) target.message( con3 );
									}									
									self.say( "O Pigmy gostou da comida, mas n�o botou um ovo." );
								}
							}
							else self.say( "Por favor, verifique se seu slot etc. est� vazio." );
						} 
						else self.say( "O Pigmy parece famindo... Por favor d� uma ra豫o na pr�xima vez." );
					}
					else self.say( "O Pigmy � um gourmet. S� alimente-o com a #bRa豫o." );
				} else {
					self.say( "O Pigmy est� satisfeito e n�o comer� mais." );
					end;
				}
			}
		}
	} else {
		pigmy_exchange;
		end;
	}
}
*/
/* Blocked by JK 2008.08.13
//아쿠아로드
script "pigmy7" {
	pigmy_reinit;

	v0 = self.askMenu( "피그미에게 어떤 행동을 하시겠습니까? \r\n#b#L0# 맛좋은 사료를 준다.#l\r\n#L1# 피그미가 할 말이 있는 것 같습니다.#l" );
	if ( v0 == 0 ) {
		qr = target.questRecord;
		con = qr.get( 9460);
		con2 = qr.get( 9461);
		wTime = currentTime;

	//	self.say(  "dd" + substring( wTime, 0, 8 ) + " " + con2 + " " + con );

		//퀘스트 초기화
		if( con == "" or con2 == "" ) {
			qr.set( 9460, substring( wTime, 0, 8 ) );
			qr.set( 9461, "000000000" );
			con = qr.get( 9460); //시간
			con2 = qr.get( 9461); //마을별코드
		} else if ( con != substring( wTime, 0, 8)) {
			qr.set( 9460, substring( wTime, 0, 8));
			qr.set( 9461, "000000000" );
			con = qr.get( 9460); //시간
			con2 = qr.get( 9461); //마을별코드
		}

		if ( con != "" ) {
			if (  con == substring( wTime, 0, 8 )) {
				if ( substring( con2, 7,1 ) != "1" ) {
					inventory = target.inventory;
					if ( inventory.itemCount( 2120008 ) >= 1 ) {		// 맛있는 사료 ID
						ret1 = self.askYesNo( "피그미가 배고픈 것 같습니다. 귀여운 피그미가 좋아하는 #b맛좋은 사료#k를 주겠습니까?" );
						if ( ret1 != 0 ) {
							nSlot = inventory.slotCount( 4 );
							nHold = inventory.holdCount( 4 );
							if ( nSlot > nHold or ( nSlot == nHold and inventory.itemCount( 4170007 ) % 100 > 0 ) ) {
								ret2 = inventory.makePigmyEgg( 2120008, 7 );
								if ( ret2 >= 1 ) {
									con3 = substring( con2, 0, 7 ) + "1" + substring( con2, 8, 1 );
									qr.set( 9461, con3 );
									self.say( "피그미가 맛있는 것을 먹고 기분이 좋았는지 알을 낳았습니다." );
								}
								else {
									con3 = substring( con2, 0, 7 ) + "1" + substring( con2, 8, 1 );
									qr.set( 9461, con3 );								
									if ( serverType == 2 ) {
										if ( target.nJob == 900 ) target.message( con3 );
									}
									self.say( "피그미가 맛있게 사료를 먹었지만 알은 낳지 않았습니다." );
								}
							}
							else self.say( "기타창이 비어 있는지 확인해주세요." );
						} 
						else self.say( "피그미가 많이 배고픈 것 같은데… 다음에 생각나면 꼭 먹을 것을 주세요." );
					}
					else self.say( "피그미는 아무 것이나 먹지 않습니다. #b맛 좋은 사료#k가 있는지 확인해주세요." );
				} else {
					self.say( "피그미는 배불러서 더 이상 먹지 않습니다." );
					end;
				}

			}
		}
	} else {
		pigmy_exchange;
		end;
	}
}


//노틸러스
script "pigmy8" {
	pigmy_reinit;

	v0 = self.askMenu( "피그미에게 어떤 행동을 하시겠습니까? \r\n#b#L0# 맛좋은 사료를 준다.#l\r\n#L1# 피그미가 할 말이 있는 것 같습니다.#l" );
	if ( v0 == 0 ) {
		qr = target.questRecord;
		con = qr.get( 9460);
		con2 = qr.get( 9461);
		wTime = currentTime;

		if ( target.nJob == 900 ) pigmy_master;
	//	self.say(  "dd" + substring( wTime, 0, 8 ) + " " + con2 + " " + con );

		//퀘스트 초기화
		if( con == "" or con2 == "" ) {
			qr.set( 9460, substring( wTime, 0, 8 ) );
			qr.set( 9461, "000000000" );
			con = qr.get( 9460); //시간
			con2 = qr.get( 9461); //마을별코드
		} else if ( con != substring( wTime, 0, 8)) {
			qr.set( 9460, substring( wTime, 0, 8));
			qr.set( 9461, "000000000" );
			con = qr.get( 9460); //시간
			con2 = qr.get( 9461); //마을별코드
		}

		if ( con != "" ) {
			if (  con == substring( wTime, 0, 8 )) {
				if ( substring( con2, 8,1 ) != "1" ) {
					inventory = target.inventory;
					if ( inventory.itemCount( 2120008 ) >= 1 ) {		// 맛있는 사료 ID
						ret1 = self.askYesNo( "피그미가 배고픈 것 같습니다. 귀여운 피그미가 좋아하는 #b맛좋은 사료#k를 주겠습니까?" );
						if ( ret1 != 0 ) {
							nSlot = inventory.slotCount( 4 );
							nHold = inventory.holdCount( 4 );
							if ( nSlot > nHold or ( nSlot == nHold and inventory.itemCount( 4170007 ) % 100 > 0 ) ) {
								ret2 = inventory.makePigmyEgg( 2120008, 9 ); //해외가 8번 썻으므로 우리는 9번씀
								if ( ret2 >= 1 ) {
									con3 = substring( con2, 0, 8 ) + "1";
									qr.set( 9461, con3 );
									if ( serverType == 2 ) {
										if ( target.nJob == 900 ) target.message( con3 );
									}
									self.say( "피그미가 맛있는 것을 먹고 기분이 좋았는지 알을 낳았습니다." );
								}
								else {
									con3 = substring( con2, 0, 8 ) + "1";
									qr.set( 9461, con3 );								
									self.say( "피그미가 맛있게 사료를 먹었지만 알은 낳지 않았습니다." );
								}
							}
							else self.say( "기타창이 비어 있는지 확인해주세요." );
						} 
						else self.say( "피그미가 많이 배고픈 것 같은데… 다음에 생각나면 꼭 먹을 것을 주세요." );
					}
					else self.say( "피그미는 아무 것이나 먹지 않습니다. #b맛 좋은 사료#k가 있는지 확인해주세요." );
				} else {
					self.say( "피그미는 배불러서 더 이상 먹지 않습니다." );
					end;
				}

			}
		}
	} else {
		pigmy_exchange;
		end;
	}
}
*/ 

//에뜨랑의 피그미 가이드
script "pigmy_guide" {
	while (1) {
		v0 = self.askMenu( "#b<Etran, o Especialista em Pigmy>#k \r  Eu preparei um guia com informa寤es detalhadas sobre o Pigmy. Isso te ajudar� a conhecer melhor os Pigmys!\r\r\n#b#L0# O que � um Pigmy?#l \r\n#b#L1# O que um Pigmy come?#l \r\n#b#L2# O que � um Ovo de Pigmy?#l" );
		if ( v0 == 0 ) {
		self.say( "O Pigmy foi criado por acidente em um dos meus experimentos m�gicos.� um animal gentil. Mas ele come MUITO..." );
		}

		if ( v0 == 1 ) {
			self.say( "O Pigmy come apenas #b#t2120008##k da Loja. Voc� precisa comprar a ra豫o da Loja e alimentar o Pigmy com ela. Ah, �! O Pigmy come apenas 5 vezes ao dia.Ent�o n�o tente aliment�-lo o tempo todo." );
		}

		if ( v0 == 2 ) {
			self.say( "#bO Ovo de Pigmy#k � um ovo que foi criado pelo Pigmy. #bAlimentar o Pigmy pode melhorar seu humor e faz�-lo botar ovos.#kExistem muitas coisas interessantes neste ovo. Mas voc� s� conseguir� quebrar a casca com um instrumento especial.Para abrir o Ovo de Pigmy, #bvoc� ter� que comprar uma incubadora#k da aba etc. na Loja de Itens." );
		}
	}
}







/*
// 헤네시스
script "pigmy0" {
	if ( compareTime( "06/05/11/09/00", currentTime ) >= 0 ) {
		inventory = target.inventory;
		if ( inventory.itemCount( 2120008 ) >= 1 ) {		// 맛있는 사료 ID
			ret1 = self.askYesNo( "피그미가 배고픈 것 같습니다. 귀여운 피그미가 좋아하는 #b맛좋은 사료#k를 주겠습니까?" );
			if ( ret1 != 0 ) {
				nSlot = inventory.slotCount( 4 );
				nHold = inventory.holdCount( 4 );
				if ( nSlot > nHold or ( nSlot == nHold and inventory.itemCount( 4170000 ) % 100 > 0 ) ) {
					ret2 = inventory.makePigmyEgg( 2120008, 0 );
					if ( ret2 >= 1 ) self.say( "피그미가 맛있는 것을 먹고 기분이 좋았는지 알을 낳았습니다." );
					else self.say( "피그미가 맛있게 사료를 먹었지만 알은 낳지 않았습니다." );
				}
				else self.say( "기타창이 비어 있는지 확인해주세요." );
			} 
			else self.say( "피그미가 많이 배고픈 것 같은데… 다음에 생각나면 꼭 먹을 것을 주세요." );
		}
		else self.say( "피그미는 아무 것이나 먹지 않습니다. #b맛 좋은 사료#k가 있는지 확인해주세요." );
	}
}

//엘리니아
script "pigmy1" {
	if ( compareTime( "06/05/11/09/00", currentTime ) >= 0 ) {
		inventory = target.inventory;
		if ( inventory.itemCount( 2120008 ) >= 1 ) {		// 맛있는 사료 ID
			ret1 = self.askYesNo( "피그미가 배고픈 것 같습니다. 귀여운 피그미가 좋아하는 #b맛좋은 사료#k를 주겠습니까?" );
			if ( ret1 != 0 ) {
				nSlot = inventory.slotCount( 4 );
				nHold = inventory.holdCount( 4 );
				if ( nSlot > nHold or ( nSlot == nHold and inventory.itemCount( 4170001 ) % 100 > 0 ) ) {
					ret2 = inventory.makePigmyEgg( 2120008, 1 );
					if ( ret2 >= 1 ) self.say( "피그미가 맛있는 것을 먹고 기분이 좋았는지 알을 낳았습니다." );
					else self.say( "피그미가 맛있게 사료를 먹었지만 알은 낳지 않았습니다." );
				}
				else self.say( "기타창이 비어 있는지 확인해주세요." );
			} 
			else self.say( "피그미가 많이 배고픈 것 같은데… 다음에 생각나면 꼭 먹을 것을 주세요." );
		}
		else self.say( "피그미는 아무 것이나 먹지 않습니다. #b맛 좋은 사료#k가 있는지 확인해주세요." );
	}
}

//커닝시티
script "pigmy2" {
	if ( compareTime( "06/05/11/09/00", currentTime ) >= 0 ) {
		inventory = target.inventory;
		if ( inventory.itemCount( 2120008 ) >= 1 ) {		// 맛있는 사료 ID
			ret1 = self.askYesNo( "피그미가 배고픈 것 같습니다. 귀여운 피그미가 좋아하는 #b맛좋은 사료#k를 주겠습니까?" );
			if ( ret1 != 0 ) {
				nSlot = inventory.slotCount( 4 );
				nHold = inventory.holdCount( 4 );
				if ( nSlot > nHold or ( nSlot == nHold and inventory.itemCount( 4170002 ) % 100 > 0 ) ) {
					ret2 = inventory.makePigmyEgg( 2120008, 2 );
					if ( ret2 >= 1 ) self.say( "피그미가 맛있는 것을 먹고 기분이 좋았는지 알을 낳았습니다." );
					else self.say( "피그미가 맛있게 사료를 먹었지만 알은 낳지 않았습니다." );
				}
				else self.say( "기타창이 비어 있는지 확인해주세요." );
			} 
			else self.say( "피그미가 많이 배고픈 것 같은데… 다음에 생각나면 꼭 먹을 것을 주세요." );
		}
		else self.say( "피그미는 아무 것이나 먹지 않습니다. #b맛 좋은 사료#k가 있는지 확인해주세요." );
	}
}

//페리온
script "pigmy3" {
	if ( compareTime( "06/05/11/09/00", currentTime ) >= 0 ) {
		inventory = target.inventory;
		if ( inventory.itemCount( 2120008 ) >= 1 ) {		// 맛있는 사료 ID
			ret1 = self.askYesNo( "피그미가 배고픈 것 같습니다. 귀여운 피그미가 좋아하는 #b맛좋은 사료#k를 주겠습니까?" );
			if ( ret1 != 0 ) {
				nSlot = inventory.slotCount( 4 );
				nHold = inventory.holdCount( 4 );
				if ( nSlot > nHold or ( nSlot == nHold and inventory.itemCount( 4170003 ) % 100 > 0 ) ) {
					ret2 = inventory.makePigmyEgg( 2120008, 3 );
					if ( ret2 >= 1 ) self.say( "피그미가 맛있는 것을 먹고 기분이 좋았는지 알을 낳았습니다." );
					else self.say( "피그미가 맛있게 사료를 먹었지만 알은 낳지 않았습니다." );
				}
				else self.say( "기타창이 비어 있는지 확인해주세요." );
			} 
			else self.say( "피그미가 많이 배고픈 것 같은데… 다음에 생각나면 꼭 먹을 것을 주세요." );
		}
		else self.say( "피그미는 아무 것이나 먹지 않습니다. #b맛 좋은 사료#k가 있는지 확인해주세요." );
	}
}

//엘나스
script "pigmy4" {
	if ( compareTime( "06/05/11/09/00", currentTime ) >= 0 ) {
		inventory = target.inventory;
		if ( inventory.itemCount( 2120008 ) >= 1 ) {		// 맛있는 사료 ID
			ret1 = self.askYesNo( "피그미가 배고픈 것 같습니다. 귀여운 피그미가 좋아하는 #b맛좋은 사료#k를 주겠습니까?" );
			if ( ret1 != 0 ) {
				nSlot = inventory.slotCount( 4 );
				nHold = inventory.holdCount( 4 );
				if ( nSlot > nHold or ( nSlot == nHold and inventory.itemCount( 4170004 ) % 100 > 0 ) ) {
					ret2 = inventory.makePigmyEgg( 2120008, 4 );
					if ( ret2 >= 1 ) self.say( "피그미가 맛있는 것을 먹고 기분이 좋았는지 알을 낳았습니다." );
					else self.say( "피그미가 맛있게 사료를 먹었지만 알은 낳지 않았습니다." );
				}
				else self.say( "기타창이 비어 있는지 확인해주세요." );
			} 
			else self.say( "피그미가 많이 배고픈 것 같은데… 다음에 생각나면 꼭 먹을 것을 주세요." );
		}
		else self.say( "피그미는 아무 것이나 먹지 않습니다. #b맛 좋은 사료#k가 있는지 확인해주세요." );
	}
}


//루디브리엄
script "pigmy5" {
	if ( compareTime( "06/05/11/09/00", currentTime ) >= 0 ) {
		inventory = target.inventory;
		if ( inventory.itemCount( 2120008 ) >= 1 ) {		// 맛있는 사료 ID
			ret1 = self.askYesNo( "피그미가 배고픈 것 같습니다. 귀여운 피그미가 좋아하는 #b맛좋은 사료#k를 주겠습니까?" );
			if ( ret1 != 0 ) {
				nSlot = inventory.slotCount( 4 );
				nHold = inventory.holdCount( 4 );
				if ( nSlot > nHold or ( nSlot == nHold and inventory.itemCount( 4170005 ) % 100 > 0 ) ) {
					ret2 = inventory.makePigmyEgg( 2120008, 5 );
					if ( ret2 >= 1 ) self.say( "피그미가 맛있는 것을 먹고 기분이 좋았는지 알을 낳았습니다." );
					else self.say( "피그미가 맛있게 사료를 먹었지만 알은 낳지 않았습니다." );
				}
				else self.say( "기타창이 비어 있는지 확인해주세요." );
			} 
			else self.say( "피그미가 많이 배고픈 것 같은데… 다음에 생각나면 꼭 먹을 것을 주세요." );
		}
		else self.say( "피그미는 아무 것이나 먹지 않습니다. #b맛 좋은 사료#k가 있는지 확인해주세요." );
	}
}

//오르비스
script "pigmy6" {
	if ( compareTime( "06/05/11/09/00", currentTime ) >= 0 ) {
		inventory = target.inventory;
		if ( inventory.itemCount( 2120008 ) >= 1 ) {		// 맛있는 사료 ID
			ret1 = self.askYesNo( "피그미가 배고픈 것 같습니다. 귀여운 피그미가 좋아하는 #b맛좋은 사료#k를 주겠습니까?" );
			if ( ret1 != 0 ) {
				nSlot = inventory.slotCount( 4 );
				nHold = inventory.holdCount( 4 );
				if ( nSlot > nHold or ( nSlot == nHold and inventory.itemCount( 4170006 ) % 100 > 0 ) ) {
					ret2 = inventory.makePigmyEgg( 2120008, 6 );
					if ( ret2 >= 1 ) self.say( "피그미가 맛있는 것을 먹고 기분이 좋았는지 알을 낳았습니다." );
					else self.say( "피그미가 맛있게 사료를 먹었지만 알은 낳지 않았습니다." );
				}
				else self.say( "기타창이 비어 있는지 확인해주세요." );
			} 
			else self.say( "피그미가 많이 배고픈 것 같은데… 다음에 생각나면 꼭 먹을 것을 주세요." );
		}
		else self.say( "피그미는 아무 것이나 먹지 않습니다. #b맛 좋은 사료#k가 있는지 확인해주세요." );
	}
}

//아쿠아로드
script "pigmy7" {
	if ( compareTime( "06/05/11/09/00", currentTime ) >= 0 ) {
		inventory = target.inventory;
		if ( inventory.itemCount( 2120008 ) >= 1 ) {		// 맛있는 사료 ID
			ret1 = self.askYesNo( "피그미가 배고픈 것 같습니다. 귀여운 피그미가 좋아하는 #b맛좋은 사료#k를 주겠습니까?" );
			if ( ret1 != 0 ) {
				nSlot = inventory.slotCount( 4 );
				nHold = inventory.holdCount( 4 );
				if ( nSlot > nHold or ( nSlot == nHold and inventory.itemCount( 4170007 ) % 100 > 0 ) ) {
					ret2 = inventory.makePigmyEgg( 2120008, 7 );
					if ( ret2 >= 1 ) self.say( "피그미가 맛있는 것을 먹고 기분이 좋았는지 알을 낳았습니다." );
					else self.say( "피그미가 맛있게 사료를 먹었지만 알은 낳지 않았습니다." );
				}
				else self.say( "기타창이 비어 있는지 확인해주세요." );
			} 
			else self.say( "피그미가 많이 배고픈 것 같은데… 다음에 생각나면 꼭 먹을 것을 주세요." );
		}
		else self.say( "피그미는 아무 것이나 먹지 않습니다. #b맛 좋은 사료#k가 있는지 확인해주세요." );
	}
}

//에뜨랑의 피그미 가이드
script "pigmy_guide" {
	if ( compareTime( "06/05/11/09/00", currentTime ) >= 0 ) {
		while (1) {
			v0 = self.askMenu( "#b<에뜨랑의 피그미 가이드>#k \r 안녕! 나는 에뜨랑이야. 여러분을 위해 피그미에 대한 여러가지 정보를 정리했어. 궁금한 것이 있으면 한번 천천히 읽어보라고~ \r\r\n#b#L0# 소환수 피그미가 무엇인가요?#l \r\n#b#L1# 소환수 피그미는 무엇을 먹나요?#l \r\n#b#L2# 피그미 에그는 무엇인가요?#l" );

			if ( v0 == 0 ) {
				self.say( "소환수 피그미는 내가 마법 실험을 하다가 실수로 태어난 소환수야. 착하고 온순한 생물체이지. 하지만 너무 많이 먹는다는 것이 단점이랄까..." );
			}

			if ( v0 == 1 ) {
				self.say( "소환수 피그미는 잡화점에서 팔고 있는 #b#t2120008##k만을 먹어. 잡화점에서 먹이를 구매하고 소환수 피그미에게 주면 돼." );
			}

			if ( v0 == 2 ) {
				self.say( "#b피그미 에그#k는 피그미가 낳은 알이야. 먹을 것을 주면 가끔 기분 좋아서 알을 낳지. #b이 알 속에는 신기한 물건들이 많이 들어있지.#k 그런데 한 가지 주의할 점은 알이 너무 단단해서 특별한 장치 없이는 열지 못해. 피그미 에그를 열기 위해서는 #b캐시샵에 들어가서 기타 탭의 게임에 있는 부화기#k라는 것을 구매해서 이 장치를 가지고 열어야 돼." );
			}
		}
	}
}
*/
