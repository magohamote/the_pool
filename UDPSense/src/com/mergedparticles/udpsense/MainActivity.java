package com.mergedparticles.udpsense;

import android.content.Context;
import android.hardware.Sensor;
import android.hardware.SensorEvent;
import android.hardware.SensorEventListener;
import android.hardware.SensorManager;
import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.support.v7.app.ActionBarActivity;
import android.view.LayoutInflater;
import android.view.Menu;
import android.view.MenuItem;
import android.view.View;
import android.view.ViewGroup;
import android.view.WindowManager;
import android.widget.Button;
import android.widget.EditText;
import android.widget.TextView;

public class MainActivity extends ActionBarActivity implements SensorEventListener {
    // Extra key for IP address
    public static final String EXTRA_ADDRESS = "com.mergedparticles.EXTRA_ADDRESS";
    
    // Extra key for port number
    public static final String EXTRA_PORT = "com.mergedparticles.EXTRA_PORT";
    
    // Extra key for accelerometer values
    public static final String EXTRA_ACCELEROMETER = "com.mergedparticles.EXTRA_ACCELEROMETER";
    
    // Constant for broadcasts
    public static final String ACTION_UI_UPDATE = "com.mergedparticles.ACTION_UI_UPDATE";
    
    // Datagram variables
    private String mAddress = null;
    private String mPort = null;

    // Sensor variables
    private SensorManager mSensorManager = null;
    private Sensor mAcceleration = null;
    
    // Used to indicate if the capture has been started.
    private boolean mIsStarted;
    
/*****************************************************************************/

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        if (savedInstanceState == null) {
            getSupportFragmentManager().beginTransaction()
                    .add(R.id.container, new CaptureFragment())
                    .commit();
        }

        mSensorManager = (SensorManager) getSystemService(Context.SENSOR_SERVICE);
        mAcceleration = mSensorManager.getDefaultSensor(Sensor.TYPE_ACCELEROMETER);

        getWindow().addFlags(WindowManager.LayoutParams.FLAG_KEEP_SCREEN_ON);

        mIsStarted = false;
    }

    @Override
    protected void onPause() {
        super.onPause();
        if (mIsStarted) {
            stopCapture((Button) findViewById(R.id.captureButton));
        }
    }
    
/*****************************************************************************/

    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        // Inflate the menu; this adds items to the action bar if it is present.
        getMenuInflater().inflate(R.menu.main, menu);
        return true;
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {
        // Handle action bar item clicks here. The action bar will
        // automatically handle clicks on the Home/Up button, so long
        // as you specify a parent activity in AndroidManifest.xml.
        int id = item.getItemId();
        if (id == R.id.action_settings) {
            return true;
        }
        return super.onOptionsItemSelected(item);
    }
    
/*****************************************************************************/

    @Override
    public void onAccuracyChanged(Sensor sensor, int accuracy) {
        // Do nothing
    }

    @Override
    public void onSensorChanged(SensorEvent event) {
        if (mAddress != null && mPort != null) {
            // Format: "TimeStamp:x:y:z"
            String displayValues = String.format("x=%.1f", event.values[0])
                    + ", y=" + String.format("%.1f", event.values[1])
                    + ", z=" + String.format("%.1f", event.values[2]);
            TextView accelerationView = (TextView) findViewById(R.id.accelerationValue);
            accelerationView.setText(displayValues);
            

            // x>0 => ???, x<0 = ???, can be ignored
            // y>0 => turn left, y<0 = turn right
            // z>0 => acceleration, z<0 = break
            String values = String.format("%.3f", event.values[0])
                    + ":" + String.format("%.3f", event.values[1])
                    + ":" + String.format("%.3f", event.values[2]);
            new SensorUdpSender().execute(values, mAddress, mPort);
        }
    }
    
/*****************************************************************************/
    
    /**
     * Called when the captureButton is tapped.
     * @param view The captureButton.
     */
    public void captureButtonListener(View view) {
        Button captureButton = (Button) view;
        if (mIsStarted) {
            this.stopCapture(captureButton);
        } else {
            this.startCapture(captureButton);
        }
    }
    
    /**
     * Starts the service that captures and sends data via UDP.
     * @param view The captureButton view.
     */
    public void startCapture(Button captureButton) {
        captureButton.setText(R.string.stop_capture);
        EditText addressView = (EditText) findViewById(R.id.addressValue);
        mAddress = addressView.getText().toString();
        EditText portView = (EditText) findViewById(R.id.portValue);
        mPort = portView.getText().toString();
        
        mSensorManager.registerListener(this, mAcceleration, SensorManager.SENSOR_DELAY_GAME);
        mIsStarted = true;
    }
    
    /**
     * Stop the sensor capture service.
     * @param view The captureButton view.
     */
    public void stopCapture(Button captureButton) {
        captureButton.setText(R.string.start_capture);

        mSensorManager.unregisterListener(this);
        mIsStarted = false;
    }
    
/*****************************************************************************/

    /**
     * Fragment display the interface to start the capture of sensor information.
     */
    public static class CaptureFragment extends Fragment {
        
        /**
         * Class constructor.
         */
        public CaptureFragment() {
        }

        @Override
        public View onCreateView(LayoutInflater inflater, ViewGroup container,
                Bundle savedInstanceState) {
            View rootView = inflater.inflate(R.layout.fragment_main, container, false);
            return rootView;
        }
    }
}
