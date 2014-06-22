package com.upokecenter.test;

import java.util.*;
import com.upokecenter.cbor.CBORObject;

  final class AppResources {

    ResourceBundle mgr;

    public AppResources(String name) {
      mgr = ResourceBundle.getBundle(name);
    }

    public CBORObject GetJSON(String name) {
      return CBORObject.FromJSONString(GetString(name));
    }

    public String GetString(String name) {
      return mgr.getString(name);
    }
  }