#Classifiers for Nomad
#Features for each scenario will be a collection of words used most often
#after removing trash words like 'the'. I don't want to base the guess
#on words that don't really have an impact on meaning.
#THIS FILE IS NOT RUN AT RUNTIME.
#This file creates pickles of classifiers for each scenario, to be pulled upon later

import nltk
import re
import pickle
import sys
from nltk.corpus import stopwords

#returns a list of tokenized words from a plain text document
def grab_tokens(file):
	with open(file, 'r', encoding = 'utf-8') as data:
		tokens = nltk.word_tokenize(data.read().lower())
		tokens = set(tokens)
	return tokens

#removes stopwords and punctuation from a token list, and adds a label, returns a list of tuples
def grab_features(tokens):
	filtered = [w for w in tokens if w not in stopwords.words('english') and re.match('\w', w) ]
	return filtered

def add_to_classifier(classSet, filename, label, all):
	featSet = {}
	templist = grab_features(grab_tokens(filename))
	for feat in all:
		if (feat in templist):
			featSet[feat] = 1
		else:
			featSet[feat] = 0
	classSet.append((featSet,label))
	return classSet
	
# def add_to_classifier(classSet, filename, label, all):
	# featSet = {}
	# templist = grab_features(grab_tokens(filename))
	# fullfile = open(filename, 'r')
	# fileString = fullfile.read()
	# fileString = fileString.split()
	# fullfile.close()
	# fullfile = open(filename, 'r')
	# for line in fullfile:
		# all.append(line.lower().rstrip())
	# for feat in all:
		# if (feat in templist):
			# featSet[feat] = fileString.count(feat)
		# else:
			# featSet[feat] = 0
	# classSet.append((featSet,label))
	# return classSet
	
def add_to_allWords(all, filename):
	templist = grab_features(grab_tokens(filename))
	for each in templist:
		all.append(each)
	fullfile = open(filename, 'r')
	for line in fullfile:
		all.append(line.lower().rstrip())
	return
#sprout tree next to [item]
if __name__ == "__main__":
	#print (sys.path[0])
	basepath = sys.path[0]
	allWords = []
	trainSet = []
	#tokens = grab_tokens('C:/Users/SidneySmall/Desktop/Nomad_Classifier/ClassifierData/giveAcorn.txt')
	#features = grab_features(tokens)
	#freqFiltered = nltk.FreqDist(w for w in filtered)
	#features = list(freqFiltered)[:8]
	#trainSet += filtered
	#--#default#--#
	# add_to_allWords(allWords, basepath + '/ClassifierData/default/createAxe.txt')
	# add_to_allWords(allWords, basepath + '/ClassifierData/default/setTimeNight.txt')
	# add_to_allWords(allWords, basepath + '/ClassifierData/default/setTimeDay.txt')
	# add_to_allWords(allWords, basepath + '/ClassifierData/default/createMountainLion.txt')
	# add_to_allWords(allWords, basepath + '/ClassifierData/default/createBunny.txt')
	# # add_to_allWords(allWords, basepath + '/ClassifierData/default/createTree.txt')
	# add_to_allWords(allWords, basepath + '/ClassifierData/default/createBear.txt')
	# add_to_classifier(trainSet, basepath + '/ClassifierData/default/createAxe.txt', 'createAxe', allWords)
	# add_to_classifier(trainSet, basepath + '/ClassifierData/default/setTimeNight.txt', 'Night', allWords)
	# add_to_classifier(trainSet, basepath + '/ClassifierData/default/setTimeDay.txt', 'Day', allWords)
	# add_to_classifier(trainSet, basepath + '/ClassifierData/default/createMountainLion.txt', 'createMountainLion', allWords)
	# add_to_classifier(trainSet, basepath + '/ClassifierData/default/createBunny.txt', 'createBunny', allWords)
	# # add_to_classifier(trainSet, basepath + '/ClassifierData/default/createTree.txt', 'createTree', allWords)
	# add_to_classifier(trainSet, basepath + '/ClassifierData/default/createBear.txt', 'createBear', allWords)
	# add_to_allWords(allWords, basepath + '/ClassifierData/playerNearTree/giveAcorn.txt')
	# add_to_allWords(allWords, basepath + '/ClassifierData/playerNearTree/fallOnPlayer.txt')
	# add_to_allWords(allWords, basepath + '/ClassifierData/playerNearTree/growTree.txt')
	# add_to_allWords(allWords, basepath + '/ClassifierData/playerNearTree/setFire.txt')
	# add_to_classifier(trainSet, basepath + '/ClassifierData/playerNearTree/giveAcorn.txt', 'giveAcorn', allWords)
	# add_to_classifier(trainSet, basepath + '/ClassifierData/playerNearTree/fallOnPlayer.txt', 'fallOnPlayer', allWords)
	# add_to_classifier(trainSet, basepath + '/ClassifierData/playerNearTree/growTree.txt', 'growTree', allWords)
	# add_to_classifier(trainSet, basepath + '/ClassifierData/playerNearTree/setFire.txt', 'setFire', allWords)
	#--#playerNearBear#--#
	# add_to_allWords(allWords, basepath + '/ClassifierData/playerNearBear/increaseHostility.txt')
	# add_to_allWords(allWords, basepath + '/ClassifierData/playerNearBear/decreaseHostility.txt')
	# add_to_allWords(allWords, basepath + '/ClassifierData/playerNearBear/killBear.txt')
	# add_to_allWords(allWords, basepath + '/ClassifierData/playerNearBear/spawnBearCub.txt')
	# add_to_allWords(allWords, basepath + '/ClassifierData/playerNearBear/increaseStrength.txt')
	# add_to_allWords(allWords, basepath + '/ClassifierData/playerNearBear/runAwayBear.txt')
	# add_to_allWords(allWords, basepath + '/ClassifierData/playerNearBear/guardTree.txt')
	# add_to_allWords(allWords, basepath + '/ClassifierData/playerNearBear/guardRock.txt')
	# add_to_classifier(trainSet, basepath + '/ClassifierData/playerNearBear/increaseHostility.txt', 'increaseHostility', allWords)
	# add_to_classifier(trainSet, basepath + '/ClassifierData/playerNearBear/decreaseHostility.txt', 'decreaseHostility', allWords)
	# add_to_classifier(trainSet, basepath + '/ClassifierData/playerNearBear/killBear.txt', 'killBear', allWords)
	# add_to_classifier(trainSet, basepath + '/ClassifierData/playerNearBear/spawnBearCub.txt', 'spawnBearCub', allWords)
	# add_to_classifier(trainSet, basepath + '/ClassifierData/playerNearBear/increaseStrength.txt', 'increaseStrength_bear', allWords)
	# add_to_classifier(trainSet, basepath + '/ClassifierData/playerNearBear/runAwayBear.txt', 'runAway_bear', allWords)
	# add_to_classifier(trainSet, basepath + '/ClassifierData/playerNearBear/guardTree.txt', 'bearGuard_tree', allWords)
	# add_to_classifier(trainSet, basepath + '/ClassifierData/playerNearBear/guardRock.txt', 'bearGuard_rock', allWords)
	#--#playerNearRabbit#--#
	# add_to_allWords(allWords, basepath + '/ClassifierData/playerNearBunny/babyBunny.txt')
	# add_to_allWords(allWords, basepath + '/ClassifierData/playerNearBunny/killerBunny.txt')
	# add_to_allWords(allWords, basepath + '/ClassifierData/playerNearBunny/runAway.txt')
	# add_to_classifier(trainSet, basepath + '/ClassifierData/playerNearBunny/babyBunny.txt', 'babyBunny', allWords)
	# add_to_classifier(trainSet, basepath + '/ClassifierData/playerNearBunny/killerBunny.txt', 'killerBunny', allWords)
	# add_to_classifier(trainSet, basepath + '/ClassifierData/playerNearBunny/runAway.txt', 'runAway_bunny', allWords)
	#--#playerNearRiver#--#
	# add_to_allWords(allWords, basepath + '/ClassifierData/playerNearRiver/createBridge.txt')
	# add_to_allWords(allWords, basepath + '/ClassifierData/playerNearRiver/createBoat.txt')
	# add_to_allWords(allWords, basepath + '/ClassifierData/playerNearRiver/partWater.txt')
	# add_to_classifier(trainSet, basepath + '/ClassifierData/playerNearRiver/createBridge.txt', 'createBridge', allWords)
	# add_to_classifier(trainSet, basepath + '/ClassifierData/playerNearRiver/createBoat.txt', 'createBoat', allWords)
	# add_to_classifier(trainSet, basepath + '/ClassifierData/playerNearRiver/partWater.txt', 'partWater', allWords)
	#--#playerNearMountainlion#--#
	add_to_allWords(allWords, basepath + '/ClassifierData/playerNearMountainLion/enrageLion.txt')
	add_to_classifier(trainSet, basepath + '/ClassifierData/playerNearMountainLion/enrageLion.txt', 'increaseStrength_lion', allWords)
	#--##--#
	print(trainSet)
	classifier = nltk.NaiveBayesClassifier.train(trainSet)
	outfile = open(basepath + '/ClassifierPickles/NearLionScenario.pickle','wb')
	pickle.dump(classifier,outfile)
	outfile.close()
	