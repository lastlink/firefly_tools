import sys
import getopt
import csv
import time
import re

# author: lastlink

# sys.exit()


def main(argv):
    inputfile = ''
    outputfile = ''
    try:
        opts, args = getopt.getopt(
            argv, "hi:o:b:", ["ifile=", "ofile=", "bank="])
    except getopt.GetoptError:
        print('test.py -i <inputfile> -o <outputfile> -b <bankname>')
        sys.exit(2)
    for opt, arg in opts:
        if opt == '-h':
            print('test.py -i <inputfile> -o <outputfile>')
            sys.exit()
        elif opt in ("-i", "--ifile"):
            inputfile = arg
        elif opt in ("-o", "--ofile"):
            outputfile = arg
        elif opt in ("-b", "--bank"):
            bank = arg
            validBanks = ['wells', 'chase']
            try:
                validBanks.index(bank)
            except ValueError:
                print(bank + " not in " + str(validBanks))
                sys.exit()
    print('Input file is "', inputfile)
    print('Output file is "', outputfile)
    start = time.time()
    print("read by line timer:")
    lineNum = 0
    with open(inputfile) as f:
        #  place the csv export name here, note when importing into sql database this will be name of table
        with open(outputfile, "w", newline='') as file:
            csv_file = csv.writer(file)
            outputFormat = ['Notes', 'Posting Date', 'Description',
                            'Amount (Debit)', 'Amount (Credit)', 'Budget', 'Balance', 'Account']
            csv_file.writerow(outputFormat)

            for line in f:
                # print(line)
                lineArr = line.split(",")
                rowResult = [''] * len(outputFormat)
                if bank == 'chase':
                    if lineNum == 0:
                        baseHeader = lineArr
                        print(baseHeader)
                    else:
                        if lineArr[baseHeader.index('Details')] == 'DEBIT':
                            rowResult[outputFormat.index(
                                'Amount (Debit)')] = lineArr[baseHeader.index('Amount')]
                        elif lineArr[baseHeader.index('Details')] == 'CREDIT':
                            rowResult[outputFormat.index(
                                'Amount (Credit)')] = lineArr[baseHeader.index('Amount')]
                        else:
                            print(
                                "Missing credit/debit:" + lineArr[baseHeader.index('Details')] + " on line:" + lineNum)

                        rowResult[outputFormat.index(
                            'Notes')] = lineArr[baseHeader.index('Details')]
                        # if(lineArr[baseHeader()])
                        # if()
                        # print
                        # if 4 in test_list_set:
                        rowResult[outputFormat.index('Description')] = re.sub(
                            "\s\s+", " ", lineArr[baseHeader.index('Description')])
                        # need to clean out junk
                        rowResult[outputFormat.index(
                            'Account')] = re.sub(
                            "((0|1)\d{1})\/((0|1|2)\d{1})", " ", rowResult[outputFormat.index('Description')])
                        rowResult[outputFormat.index(
                            'Posting Date')] = lineArr[baseHeader.index('Posting Date')]
                        rowResult[outputFormat.index(
                            'Balance')] = lineArr[baseHeader.index('Balance')]

                        # budget logic
                        pass
                elif bank == 'wells':
                    if lineNum == 0:
                        baseHeader = ['Posting Date','Amount','Star','Blank','Description']
                        print(baseHeader)

                    rowResult[outputFormat.index('Description')] = re.sub(
                            "\s\s+", " ", lineArr[baseHeader.index('Description')].replace('"', '').rstrip())
                    if float(lineArr[baseHeader.index('Amount')].replace('"', '')) > 0:
                        rowResult[outputFormat.index(
                                'Amount (Credit)')] = lineArr[baseHeader.index('Amount')].replace('"', '')
                    else:
                        rowResult[outputFormat.index(
                                'Amount (Debit)')] = abs(float(lineArr[baseHeader.index('Amount')].replace('"', '')))
                                
                    rowResult[outputFormat.index(
                            'Posting Date')] = lineArr[baseHeader.index('Posting Date')].replace('"', '')
                    
                    rowResult[outputFormat.index(
                            'Account')] = re.sub(
                            "((0|1)\d{1})\/((0|1|2)\d{1})", " ", rowResult[outputFormat.index('Description')])
                    pass
                else:
                    print('bank not implemented')
                    sys.exit()

                csv_file.writerow(rowResult)

                # csv_file.writerow(lineArr)
                # csv_file.writerow(rowResult)

                # print(baseHeader)
                # print(line[0])
                lineNum += 1

    end = time.time()
    print(end - start, "line num", lineNum)


# call main function
if __name__ == "__main__":
    main(sys.argv[1:])
