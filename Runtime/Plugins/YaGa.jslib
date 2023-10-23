const YaGa_Adv = {

	YaGa_showFullscreenAdv: function () {
		ysdk.adv.showFullscreenAdv({
			callbacks: {
				onOpen: function () {
					myGameInstance.SendMessage('YaGa', 'Full_OnOpen');
				},
				onClose: function (wasShown) {
					if (wasShown)
						myGameInstance.SendMessage('YaGa', 'Full_OnClose', 1);
					else
						myGameInstance.SendMessage('YaGa', 'Full_OnClose', 0);
				},
				onError: function (error) {
					myGameInstance.SendMessage('YaGa', 'Full_OnError');
				},
				onOffline: function () {
					myGameInstance.SendMessage('YaGa', 'Full_OnOffline');
				}
			}
		})
	},

	YaGa_showRewardedVideo: function () {
		ysdk.adv.showRewardedVideo({
			callbacks: {
				onOpen: () => {
					myGameInstance.SendMessage('YaGa', 'Reward_OnOpen');
				},
				onRewarded: () => {
					myGameInstance.SendMessage('YaGa', 'Reward_OnRewarded');
				},
				onClose: () => {
					myGameInstance.SendMessage('YaGa', 'Reward_OnClose');
				},
				onError: (error) => {
					myGameInstance.SendMessage('YaGa', 'Reward_OnError');
				}
			}
		})
	}

};

const YaGa_Player = {

	YaGa_getCachedData: function () {
		var bufferSize = lengthBytesUTF8(playerData) + 1;
		var buffer = _malloc(bufferSize);
		stringToUTF8(playerData, buffer, bufferSize);
		return buffer;
	},

	YaGa_playerGetData: function () {
		player.getData().then(dataJson => {
			let dataString = JSON.stringify(dataJson);
			playerData = dataString;
			myGameInstance.SendMessage('YaGa', 'OnGetPlayerData', dataString);
		});
	},

	YaGa_playerSetData: function (dataObj) {
		let dataString = UTF8ToString(dataObj);
		let dataJson = JSON.parse(dataString);
		player.setData(dataJson).then(() => {
			playerData = dataString;
		});
	},

	YaGa_playerGetStats: function () {
		player.getStats().then(dataJson => {
			let dataString = JSON.stringify(dataJson);
			myGameInstance.SendMessage('YaGa', 'OnGetPlayerStats', dataString);
		});
	},

	YaGa_playerSetStats: function (dataObj) {
		let dataString = UTF8ToString(dataObj);
		let dataJson = JSON.parse(dataString);
		player.setStats(dataJson);
	}

};

const YaGa_Leaderboard = {

	YaGa_getLeaderboardDescription: function(leaderboardName) {
		var lbName = UTF8ToString(leaderboardName);
		ysdk.getLeaderboards().then(lb => {
			lb.getLeaderboardDescription(lbName).then(res => {
				result = {
					// "appID": res.appID,
					"isDefault": res.default,
					"isInvertSortOrder": res.description.invert_sort_order,
					"decimalOffset": res.description.score_format.options.decimal_offset,
					"type": (res.description.score_format.type === "numeric" ? 0 : res.description.score_format.type === "time" ? 1 : -1),
					"name": res.name,
					"title": res.title
				}
				myGameInstance.SendMessage('YaGa', 'OnGetDescription', JSON.stringify(result));
			});
		});
	},

	YaGa_getLeaderboardPlayerEntry: function(leaderboardName, avatarSize) {
		var lbName = UTF8ToString(leaderboardName);
		ysdk.getLeaderboards().then(lb => {
			lb.getLeaderboardPlayerEntry(lbName).then(res => {
				res.player = {
					"publicName": res.player.publicName,
					"uniqueID": res.player.uniqueID,
					"avatarURL": (avatarSize !== "none" && res.player.scopePermissions.avatar === "allow")
						? res.player.getAvatarSrc(avatarSize)
						: ""
				}
				myGameInstance.SendMessage('YaGa', 'OnGetPlayerEntry', JSON.stringify(res));
			}).catch(err => {
				if (err.code === 'LEADERBOARD_PLAYER_NOT_PRESENT') {
					myGameInstance.SendMessage('YaGa', 'OnPlayerNotPresentError');
				}
			});
		});
	},

	YaGa_setLeaderboardScore: function(leaderboardName, score, extraData) {
		var lbName = UTF8ToString(leaderboardName);
		var exData = UTF8ToString(extraData);
		ysdk.getLeaderboards().then(lb => {
			lb.setLeaderboardScore(lbName, score, exData);
		});
	}

};

const YaGa_Feedback = {

	YaGa_requestReview : function (){
		ysdk.feedback.canReview().then(({ value, reason }) => {
			if (value) {
				ysdk.feedback.requestReview().then(({ feedbackSent }) => {
					myGameInstance.SendMessage('YaGa', 'OnRequestReview', feedbackSent);
				})
			} else {
				myGameInstance.SendMessage('YaGa', 'OnCanReviewFailed', reason);
			}
		});
	}

};

const YaGa_Device = {

	YaGa_getDeviceType: function () {
		var type = ysdk.deviceInfo.type;

		var bufferSize = lengthBytesUTF8(type) + 1;
		var buffer = _malloc(bufferSize);
		stringToUTF8(type, buffer, bufferSize);
		return buffer;
	},

	YaGa_getMobileType: function () {
		var type = "None";
		if (/iPhone|iPad|iPod/i.test(navigator.userAgent))
			type = "iOS";
		if (/Android/i.test(navigator.userAgent))
			type = "Android";

		var bufferSize = lengthBytesUTF8(type) + 1;
		var buffer = _malloc(bufferSize);
		stringToUTF8(type, buffer, bufferSize);
		return buffer;
	}

};

const YaGa_Environment = {

	YaGa_getEnvironment: function () {
		var env = JSON.stringify(ysdk.environment);

		var bufferSize = lengthBytesUTF8(env) + 1;
		var buffer = _malloc(bufferSize);
		stringToUTF8(env, buffer, bufferSize);
		return buffer;
	}

};

const YaGa_Console = {

	YaGa_consoleLog: function (logMessage, logType) {
		let message = "%c❂%c " + UTF8ToString(logMessage);
		const labelStyle = 'color: orange';
		const textStyle = 'color: unset';
		switch (logType) {
			case 1:
				console.info(message, labelStyle, textStyle);
				break;
			case 2:
				console.warn(message, labelStyle, textStyle);
				break;
			case 3:
				console.error(message, labelStyle, textStyle);
				break;
			case 0:
			default:
				console.log(message, labelStyle, textStyle);
				break;
		}
	},

};

const YaGa_Utils = {

	YaGa_getAddress : function () {
		let address;
		const re = /.*yandex\..*\/games\/.*app\/\d+.*/;

		try { address = document.referrer; } catch {}
		if (!re.test(address)) {
			try { address = document.location.href; } catch {}
			if (!re.test(address)) {
				address = `https://yandex.${ysdk.environment.i18n.tld}/games/app/${ysdk.environment.app.id}`;
			}
		}

		var bufferSize = lengthBytesUTF8(address) + 1;
		var buffer = _malloc(bufferSize);
		stringToUTF8(address, buffer, bufferSize);
		return buffer;
	},

	YaGa_loadingAPIReady : function () {
		ysdk.features.LoadingAPI.ready();
	},

	YaGa_ymReachGoal : function (target) {
		ym(yaMetrikaCounterID, 'reachGoal', UTF8ToString(target));
	}

};

mergeInto(LibraryManager.library, YaGa_Adv);
mergeInto(LibraryManager.library, YaGa_Player);
mergeInto(LibraryManager.library, YaGa_Leaderboard);
mergeInto(LibraryManager.library, YaGa_Feedback);
mergeInto(LibraryManager.library, YaGa_Device);
mergeInto(LibraryManager.library, YaGa_Environment);
mergeInto(LibraryManager.library, YaGa_Console);
mergeInto(LibraryManager.library, YaGa_Utils);
