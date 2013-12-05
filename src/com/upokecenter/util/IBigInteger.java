package com.upokecenter.util;

public class IBigInteger {

	public static final IBigInteger ONE = null;
	public static final IBigInteger TEN = null;
	public static final IBigInteger ZERO = null;

	public static IBigInteger valueOf(long valueA) {
		// TODO Auto-generated method stub
		return null;
	}

	public IBigInteger(byte[] reverseBytes) {
		// TODO Auto-generated constructor stub
	}

	public IBigInteger abs() {
		return this.signum()<0 ? negate() : this;
	}
	public IBigInteger add(IBigInteger one2) {
		// TODO Auto-generated method stub
		return null;
	}

	public int compareTo(IBigInteger valueOf) {
		// TODO Auto-generated method stub
		return 0;
	}

	public IBigInteger divide(IBigInteger bigpow) {
		// TODO Auto-generated method stub
		return null;
	}

	public IBigInteger[] divideAndRemainder(IBigInteger divisor) {
		// TODO Auto-generated method stub
		return 0;
	}

	public IBigInteger gcd(IBigInteger den) {
		// TODO Auto-generated method stub
		return null;
	}

	public int intValue() {
		// TODO Auto-generated method stub
		return 0;
	}

	public long longValue() {
		// TODO Auto-generated method stub
		return 0;
	}

	public IBigInteger multiply(IBigInteger findPowerOfTenFromBig) {
		// TODO Auto-generated method stub
		return null;
	}

	public IBigInteger negate() {
		// TODO Auto-generated method stub
		return null;
	}

	public IBigInteger pow(int i) {
		// TODO Auto-generated method stub
		return null;
	}

	public IBigInteger remainder(IBigInteger valueOf) {
		return (divideAndRemainder(valueOf))[0];
	}

	public IBigInteger shiftLeft(int i) {
		// TODO Auto-generated method stub
		return null;
	}

	public IBigInteger shiftRight(int i) {
		// TODO Auto-generated method stub
		return null;
	}

	public int signum() {
		// TODO Auto-generated method stub
		return 0;
	}

	public IBigInteger subtract(IBigInteger valueOf) {
		// TODO Auto-generated method stub
		return null;
	}

	public boolean testBit(int i) {
		// TODO Auto-generated method stub
		return false;
	}

	public byte[] toByteArray() {
		// TODO Auto-generated method stub
		return null;
	}

}
