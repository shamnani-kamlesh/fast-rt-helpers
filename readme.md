# Fast Runtime Type Helpers

## Description

This library contains set of helper methods that provide fast runtime access to object's properties and fields.
Reading/writing data works more than 5x times faster than the classic reflection using GetProperty() -> GetValue / SetValue().

## Usage

### Type-level accessors

**Generic version**

```
var accessor = TypeAccessor.GetMemberAccessor(x => x.MemberName);
var value = accessor.GetValue(obj);
accessor.SetValue(obj, newValue);
```

**Basic version**
```
var accessor = TypeAccessor.GetMemberAccessor(typeof(T), "MemberName");
var value = accessor.GetValue(obj);
accessor.SetValue(obj, newValue);
```

### Object-level accessors

*Implemented as set of extension methods*

**Generic version**

```
SomeRealType obj = ...; //has a member (property or field) "MemberName"
var objectAccessor = obj.GetObjectMemberAccessor(x => x.MemberName);
objectAccessor.Value = newValue;
```

**Basic version**

```
object obj = ...;
var objectAccessor = obj.GetObjectMemberAccessor("MemberName");
var value = objectAccessor.GetValue();
objectAccessor.SetValue(newValue);
```

## TODO
1. dynamic objects support
2. new Type generation from dictionary<string, Type> with indexers by id & name
3. build delegates for method calls
