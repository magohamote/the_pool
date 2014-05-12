package com.mergedparticles.udpsense;

import java.io.IOException;
import java.net.DatagramPacket;
import java.net.DatagramSocket;
import java.net.InetAddress;
import java.net.SocketException;
import java.net.UnknownHostException;

import android.os.AsyncTask;

public class SensorUdpSender extends AsyncTask<String, Void, Void> {
    
    private DatagramSocket mUdpSocket = null;
    
    @Override
    protected Void doInBackground(String... values) {
        if (values.length < 3) {
            System.err.println("Missing arguments");
            return null;
        }
        
        InetAddress address;
        try {
            address = InetAddress.getByName(values[1]);
        } catch (UnknownHostException e1) {
            System.err.println("Unknown host");
            return null;
        }
        int port = Integer.parseInt(values[2]);
        
        String timestampedValue = 
                System.currentTimeMillis()
                + ":" + values[0];
        byte[] data = timestampedValue.getBytes();
        DatagramPacket sensingPacket = new DatagramPacket(data, data.length, address, port);
        try {
            if (mUdpSocket == null) {
                mUdpSocket = new DatagramSocket();
            }
            mUdpSocket.send(sensingPacket);
        } catch (SocketException e) {
            mUdpSocket = null;
            System.err.println("Couldn't open socket");
        } catch (IOException e) {
            System.err.println("IOException");
        }
        return null;
    }
}
