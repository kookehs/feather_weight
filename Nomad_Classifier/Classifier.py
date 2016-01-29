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
	
def add_to_allWords(all, filename):
	templist = grab_features(grab_tokens(filename))
	for each in templist:
		all.append(each)
	return

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
	#add_to_allWords(allWords, basepath + '/ClassifierData/default/createAxe.txt')
	#add_to_allWords(allWords, basepath + '/ClassifierData/default/createTree.txt')
	#add_to_classifier(trainSet, basepath + '/ClassifierData/default/createAxe.txt', 'createAxe', allWords)
	#add_to_classifier(trainSet, basepath + '/ClassifierData/default/createTree.txt', 'createTree', allWords)
	#--#playerNearTree#--#
	add_to_allWords(allWords, basepath + '/ClassifierData/playerNearTree/giveAcorn.txt')
	add_to_allWords(allWords, basepath + '/ClassifierData/playerNearTree/fallOnPlayer.txt')
	add_to_allWords(allWords, basepath + '/ClassifierData/playerNearTree/growTree.txt')
	add_to_classifier(trainSet, basepath + '/ClassifierData/playerNearTree/giveAcorn.txt', 'giveAcorn', allWords)
	add_to_classifier(trainSet, basepath + '/ClassifierData/playerNearTree/fallOnPlayer.txt', 'fallOnPlayer', allWords)
	add_to_classifier(trainSet, basepath + '/ClassifierData/playerNearTree/growTree.txt', 'growTree', allWords)
	#--#playerNearBear#--#
	#--#playerNearRabbit#--#
	#--#playerNearRiver#--#
	#--##--#
	print(trainSet)
	classifier = nltk.NaiveBayesClassifier.train(trainSet)
	outfile = open(basepath + '/ClassifierPickles/playerNearTree.pickle','wb')
	pickle.dump(classifier,outfile)
	outfile.close()
	