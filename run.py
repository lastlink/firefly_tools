import sys
import getopt
import csv
import time
import re
import configparser

# author: lastlink

settings = configparser.ConfigParser()
settings._interpolation = configparser.ExtendedInterpolation()
settings.read('settings.ini')


def main(argv):
    inputfile = ''
    outputfile = ''
    bank = ''
    validBanks = ['wells', 'chase']
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
            try:
                validBanks.index(bank)
            except ValueError:
                print(bank + " not in " + str(validBanks))
                sys.exit()
    if bank is None or bank == '':
        bank = settings.get('CurrentBank', 'bank')
        try:
            validBanks.index(bank)
        except ValueError:
            print(bank + " not in " + str(validBanks))
            sys.exit()
    print('Input file is "', inputfile)
    print('Output file is "', outputfile)
    print('Bank type is "'+bank)
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
                        rowResult[outputFormat.index('Description')] = re.sub(
                            "\s\s+", " ", lineArr[baseHeader.index('Description')])
                        # need to clean out junk
                        rowResult[outputFormat.index(
                            'Account')] = cleanAccount(rowResult[outputFormat.index('Description')])
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
                            'Account')] = cleanAccount(rowResult[outputFormat.index('Description')])
                    pass
                else:
                    print('bank not implemented')
                    sys.exit()

                rowResult[outputFormat.index(
                            'Budget')] = determineBudget(rowResult[outputFormat.index('Account')])

                csv_file.writerow(rowResult)
                lineNum += 1

    end = time.time()
    print(end - start, "line num", lineNum)

def cleanAccount(account):
    tmpAccount = account
    # remove m/d
    tmpAccount = re.sub("((0|1)\d{1})\/((0|1|2)\d{1})", " ", tmpAccount)

    # remove double spaces
    tmpAccount = re.sub("\s\s+", " ", tmpAccount)

    for accountType in settings.get('Account', 'accounts').split(','):
        accountSearch = [element.upper() for element in settings.get('Account', accountType+'_search').split(',')]
        if any(word in tmpAccount.upper() for word in accountSearch):
            return accountType
    # could possibly do matching from setting ini as well, but would be better to clean unique ideas and do a map

    return tmpAccount


def determineBudget(account):

    for category in settings.get('Budget', 'categories').split(','):
        if settings.has_option('Budget', category+'_search'):
            # make array uppercase
            categorySearch = [element.upper() for element in settings.get('Budget', category+'_search').split(',')]
            if any(word in account.upper() for word in categorySearch):
                return settings.get('Budget', category+'_name') if settings.has_option('Budget', category+'_name') else category
    return ''

# sys.exit()

# call main function
if __name__ == "__main__":
    main(sys.argv[1:])
