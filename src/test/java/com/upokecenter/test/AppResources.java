package com.upokecenter.test;

import java.util.*;
import com.upokecenter.cbor.CBORObject;

  class AppResources {

    ResourceBundle mgr;

    public AppResources(string name) {
      mgr = new ResourceBundle(name);
    }

    public CBORObject GetJSON(string name) {
      return CBORObject.FromJSONString(mgr.getString(name));
    }

    public string GetString(string name) {
      return mgr.getString(name);
    }
  }