# -*- coding: utf-8 -*-
# Generated by the protocol buffer compiler.  DO NOT EDIT!
# source: filelist.proto
"""Generated protocol buffer code."""
from google.protobuf.internal import builder as _builder
from google.protobuf import descriptor as _descriptor
from google.protobuf import descriptor_pool as _descriptor_pool
from google.protobuf import symbol_database as _symbol_database
# @@protoc_insertion_point(imports)

_sym_db = _symbol_database.Default()




DESCRIPTOR = _descriptor_pool.Default().AddSerializedFile(b'\n\x0e\x66ilelist.proto\x12\x05Proto\"-\n\x0f\x46ileListRequest\x12\x1a\n\x05\x46iles\x18\x01 \x03(\x0b\x32\x0b.Proto.File\"\x14\n\x04\x46ile\x12\x0c\n\x04Path\x18\x01 \x01(\t\"R\n\tImageFile\x12\x19\n\x04\x46ile\x18\x01 \x01(\x0b\x32\x0b.Proto.File\x12\x19\n\x04Size\x18\x02 \x01(\x0b\x32\x0b.Proto.Size\x12\x0f\n\x07\x43ontent\x18\x03 \x01(\x0c\"%\n\x04Size\x12\r\n\x05Width\x18\x01 \x01(\x05\x12\x0e\n\x06Height\x18\x02 \x01(\x05\"\x0e\n\x0cSendNextFileB\x08\xaa\x02\x05Protob\x06proto3')

_builder.BuildMessageAndEnumDescriptors(DESCRIPTOR, globals())
_builder.BuildTopDescriptorsAndMessages(DESCRIPTOR, 'filelist_pb2', globals())
if _descriptor._USE_C_DESCRIPTORS == False:

  DESCRIPTOR._options = None
  DESCRIPTOR._serialized_options = b'\252\002\005Proto'
  _FILELISTREQUEST._serialized_start=25
  _FILELISTREQUEST._serialized_end=70
  _FILE._serialized_start=72
  _FILE._serialized_end=92
  _IMAGEFILE._serialized_start=94
  _IMAGEFILE._serialized_end=176
  _SIZE._serialized_start=178
  _SIZE._serialized_end=215
  _SENDNEXTFILE._serialized_start=217
  _SENDNEXTFILE._serialized_end=231
# @@protoc_insertion_point(module_scope)