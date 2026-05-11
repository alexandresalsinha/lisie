package com.example.spirostockmanagementandroid;

//import te me.tests.R;



import org.w3c.dom.Element;
import org.w3c.dom.NodeList;

import android.os.Bundle;
import android.app.Activity;
import android.content.Intent;
import android.view.Menu;
import android.view.View;
import android.view.View.OnClickListener;
import android.widget.Button;
import android.widget.TextView;

public class MainActivity extends Activity {

	Button button_AddToShoppingCart = null;
	
	String WichListToAddInsertedBarCode;
	
    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);
        
        button_AddToShoppingCart = (Button) findViewById(R.id.button_AddToShoppingCart);
		button_AddToShoppingCart.setOnClickListener(button_AddToShoppingCart_OnClickListener);
		
		Button _button_ViewShoppingCart = (Button) findViewById(R.id.button_ViewShoppingCart);
		_button_ViewShoppingCart.setOnClickListener(button_ViewShoppingCart_OnClickListener);
		
		Button _button_ViewInventory = (Button) findViewById(R.id.button_ViewInventory);
		_button_ViewInventory.setOnClickListener(button_ViewInventory_OnClickListener);
		
		
		Button _button_AddToInventory = (Button)findViewById(R.id.button_AddToInventory);
		_button_AddToInventory.setOnClickListener(button_AddToInventory_OnClickListener);
		
		//See if barcode exists on database
//		GlobalFunctions _gb = new GlobalFunctions();
//    	NodeList _nd =  _gb.GetNodesOfXpath("//Product[BarCode='7622300465513']");
//    	
//    	//if exists show Add To Shopping card or Inventory Activity
//    	if (_nd.getLength() > 0) 
//    	{
//    		//String _productId = _nd.item(0).getTextContent();
//    		String _productId = XmlFunctions.getValue(_nd.item(0), "Id");
//    		Intent startNewActivityOpen = new Intent(
//					MainActivity.this, AddProductToList.class);
//			startNewActivityOpen.putExtra("productId", _productId);
//			//startNewActivityOpen.putExtra("listToAdd", GlobalVariables.WichListToAddNewProduct);
//			startNewActivityOpen.putExtra("listToAdd", "in");
//			startActivityForResult(startNewActivityOpen, 0);
//			//return;
//		}
    	
//		String _barCode = "7622300465513";
//		Intent startNewActivityOpen = new Intent(
//				MainActivity.this, AddProductNew.class);
//		startNewActivityOpen.putExtra("barCode", _barCode);
//		startActivityForResult(startNewActivityOpen, 0);
    }

    final OnClickListener button_ViewShoppingCart_OnClickListener = new OnClickListener()
    {
		public void onClick(View arg0) {
			Intent startNewActivityOpen = new Intent(
					MainActivity.this, ViewProductsList.class);
			startNewActivityOpen.putExtra("listToOpen", "out");
			startActivityForResult(startNewActivityOpen, 0);
		}
    };
    
    final OnClickListener button_ViewInventory_OnClickListener = new OnClickListener()
    {
		public void onClick(View arg0) {
			Intent startNewActivityOpen = new Intent(
					MainActivity.this, ViewProductsList.class);
			startNewActivityOpen.putExtra("listToOpen", "in");
			startActivityForResult(startNewActivityOpen, 0);
		}
    };
    
    
    final OnClickListener button_AddToInventory_OnClickListener = new OnClickListener()
    {
		public void onClick(View arg0) {
			GlobalVariables.WichListToAddNewProduct = "in";
        	
        	Intent intent = new Intent("com.google.zxing.client.android.SCAN");
        	intent.putExtra("com.google.zxing.client.android.SCAN.SCAN_MODE", "QR_CODE_MODE");
			startActivityForResult(intent, 0);
        	
		}
    };
    
  //On click listener for button1
    final OnClickListener button_AddToShoppingCart_OnClickListener = new OnClickListener() {
        public void onClick(final View v) {
        	GlobalVariables.WichListToAddNewProduct = "out";
        	
        	Intent intent = new Intent("com.google.zxing.client.android.SCAN");
        	intent.putExtra("com.google.zxing.client.android.SCAN.SCAN_MODE", "QR_CODE_MODE");
			startActivityForResult(intent, 0);
        }
    };
    

	//BarCode Scanner
	public void onActivityResult(int requestCode, int resultCode, Intent intent) {
		if (requestCode == 0) {
			if (resultCode == RESULT_OK) {
				String contents = intent.getStringExtra("SCAN_RESULT");
				String format = intent.getStringExtra("SCAN_RESULT_FORMAT");

				//See if barcode exists on database
				GlobalFunctions _gb = new GlobalFunctions();
		    	NodeList _nd =  _gb.GetNodesOfXpath("//Product[BarCode='" + contents + "']");
		    	
		    	//if exists show Add To Shopping card or Inventory Activity
		    	if (_nd.getLength() > 0) 
		    	{
		    		//String _productId = _nd.item(0).getTextContent();
		    		String _productId = XmlFunctions.getValue(_nd.item(0), "Id");
		    		Intent startNewActivityOpen = new Intent(
							MainActivity.this, AddProductToList.class);
					startNewActivityOpen.putExtra("productId", _productId);
					startNewActivityOpen.putExtra("listToAdd", GlobalVariables.WichListToAddNewProduct);
					startActivityForResult(startNewActivityOpen, 0);
					//return;
				}
				//If it doesn´t show the add new product with the barcode argument, and wich list to add on first insert product
		    	else
		    	{
		    		String _barCode = contents;
		    		Intent startNewActivityOpen = new Intent(
							MainActivity.this, AddProductNew.class);
					startNewActivityOpen.putExtra("barCode", _barCode);
					startNewActivityOpen.putExtra("listToAdd", GlobalVariables.WichListToAddNewProduct);
					startActivityForResult(startNewActivityOpen, 0);
					//return;
		    	}
//				setContentView(R.layout.add_product_to_list);
//				TextView _TextView = (TextView) findViewById(R.id.textView_EnteredBarCode);
//				_TextView.setText(contents);
				// textView1
				// Handle successful scan
			} else if (resultCode == RESULT_CANCELED) {
				// Handle cancel
			}
		}
    }
	
    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        getMenuInflater().inflate(R.menu.activity_main, menu);
        return true;
    }
}
