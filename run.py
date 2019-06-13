import sys, getopt, csv, time

# author: lastlink

def main(argv):
    inputfile = ''
    outputfile = ''
    try:
        opts, args = getopt.getopt(argv,"hi:o:b:",["ifile=","ofile=","bank="])
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
            validBanks = ['wells','chase']
            try:
                validBanks.index(bank)
            except ValueError:
                print(bank + " not in " + str(validBanks))
                sys.exit()
    print('Input file is "', inputfile)
    print('Output file is "', outputfile)
    start = time.time()
    print("read by line timer:")
    lineNum=0
    with open(inputfile) as f:
        #  place the csv export name here, note when importing into sql database this will be name of table
        with open(outputfile, "w") as file:
            csv_file = csv.writer(file)
            csv_file.writerow(['Details','Posting Date','Description','Amount (Debit)','Amount (Credit)','Budget','Balance','Account']) 
        
            # baseHeader
        # + item['fields'].values())
            for line in f:
                # print(line)
                if lineNum == 0:
                    baseHeader = line.split(",")
                    print(baseHeader)
                else: 
                    lineArr = line.split(",")
                    print(lineArr[0])
                    if bank == 'chase':
                        
                        pass
                    elif bank == 'wells':
                        pass
                    else:
                        print('bank not implemented')
                        sys.exit()
                    # switch
                    # rowResult =
                    csv_file.writerow(lineArr) 
                    # csv_file.writerow(rowResult) 

                # print(baseHeader)
                # print(line[0])
                lineNum+=1
    
    end = time.time()
    print(end - start, "line num", lineNum)

# call main function
if __name__ == "__main__":
   main(sys.argv[1:])