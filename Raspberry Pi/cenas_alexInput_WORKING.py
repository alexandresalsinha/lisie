from firebase import Firebase

import datetime


print 'barcode scanner tests'

while True:

        barCode = raw_input('Pass a code in the bar code reader: ')

        print barCode

        ##url = "https://brilliant-torch-9476.firebaseio.com/SpiroStockManagement/%r" % r

        url = "https://brilliant-torch-9476.firebaseio.com/SpiroStockManagement/"

        p = Firebase(url, auth_token="D5xCysnt6IfFe9FFLG4WUYCcotv9H4VlmISwSVIP")
        
	now = datetime.datetime.now()
        
	p.post({'date': str(now), 'barCode': barCode, 'isRegistered' : 'false'})
        
	print "Bar code sent to Firebase! |w|"
