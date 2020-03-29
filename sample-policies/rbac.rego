package app.rbac

# By default, deny requests.
default allow = false

# Allow admins to do anything.
allow {
	user_is_admin
}

# user_is_admin is true if...
user_is_admin {

	# for some `i`...
	some i

	# "admin" is the `i`-th element in the user->role mappings for the identified user.
	data.user_roles[input.user][i] == "admin"
}
