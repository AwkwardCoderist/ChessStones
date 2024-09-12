import csv
from googletrans import Translator

translator = Translator(service_urls=['translate.googleapis.com'])

path = input("Enter input file path: ")
savePath = input("Enter output file path: ")

translated = []

with open(path, newline='', encoding="utf8") as csvfile:
    localize = csv.reader(csvfile, delimiter = '\n')
    for row in localize:
        row = row[0].split(',', 1)
        row[1] = translator.translate(row[1], dest='en').text
        translated.append([row[0], row[1]])

with open(savePath, encoding="utf8", mode='w', newline='') as saveFile:
    writer = csv.writer(saveFile)
    for row in translated:
        print(row)
        writer.writerow(row)
    

    saveFile.close()

