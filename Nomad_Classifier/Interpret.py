#This will try and make educated guesses about the incoming strings
#when you call this file you need to have a few things done beforehand.
#in this file's directory, have a chat file you are going to have classified
#you also need to have the name of the scenario determined
#so it would be running this file with the scenario followed by the chat file
#output will be put int he same directory in a little file called "guess.txt"
# as an example, I am currently calling it with this line in CMD
#	C:\Python34\python.exe C:\Users\SidneySmall\Desktop\Nomad_Classifier\Interpret.py 0 /test.txt

import nltk
import re
import sys
import pickle
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
	basepath = sys.path[0]
	scenario = sys.argv[1]
	chatfile = sys.argv[2]
	f = open(basepath + '/ClassifierPickles/'+ scenario + '.pickle','rb')
	classifier = pickle.load(f)
	f.close()
	testData = []
	allWords = []
	add_to_allWords(allWords, chatfile)
	add_to_classifier(testData, chatfile, '0', allWords)
	print (testData[0][0])
	guess = classifier.classify(testData[0][0])
	print (guess)
	myGuess = open(basepath + '/guess.txt', 'w')
	myGuess.write(guess)
	myGuess.close()
	print(classifier.labels())
	chatfile = open(chatfile)
	#print (chatfile)
	for line in chatfile:
		#print(line)
		#print(line.split())
		influence = float(line.split()[0])
		print (influence)
		testData = []
		allWords = []
		add_to_allWords(allWords, line)
		add_to_classifier(testData, line, '0', allWords)
		guess = classifier.classify(testData[0][0])
		guessprob = classifier.prob_classify(testData[0][0]).prob(guess)
		print(guessprob)
		print (guess)
		if (guess not in decision):
			decision[guess] = 0
		if (guessprob > .25):
			decision[guess] = decision[guess] + influence
	print (decision)
	myGuess = open(basepath + '/guess.txt', 'w')
	myGuess.write(max(decision.keys(), key=lambda key: decision[key]))
	myGuess.close()
	c = open(basepath + '/check.txt', 'w')
	c.close()
